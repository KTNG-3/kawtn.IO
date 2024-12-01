﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace kawtn.IO.Konfig
{
    class ParserValue
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }

    class ParserTable
    {
        public required string Key { get; set; }
        public List<ParserValue> Values { get; set; } = new();
    }

    class ParserList
    {
        public required string Key { get; set; }
        public List<string> Values { get; set; } = new();
    }

    static class Parser
    {
        static int Int(string value)
        {
            return int.Parse(value);
        }

        static float Float(string value)
        {
            return float.Parse(value);
        }

        static bool Bool(string value)
        {
            if (int.TryParse(value, out int vInt))
            {
                if (vInt == 0) return false;
                if (vInt == 1) return true;
            }

            return bool.Parse(value);
        }

        static object[]? List(object obj)
        {
            Type? elementType = obj.GetType().GetElementType();
            if (elementType == null) return null;

            if (elementType == typeof(string))
            {
                return (string[])obj;
            }
            else if (elementType == typeof(int))
            {
                return Array.ConvertAll((int[])obj, x => (object)x);
            }
            else if (elementType == typeof(float))
            {
                return Array.ConvertAll((float[])obj, x => (object)x);
            }
            else if (elementType == typeof(bool))
            {
                return Array.ConvertAll((bool[])obj, x => (object)x);
            }

            return (object[])obj;
        }

        static string? Value(object value)
        {
            string str = value.ToString() ?? string.Empty;
            Type type = value.GetType();

            if (type == typeof(string))
            {
                if (str.Contains(' '))
                {
                    return $"\"{value}\"";
                }
                else
                {
                    return str;
                }
            }

            if (type == typeof(bool))
            {
                return str.ToLower();
            }

            if (type == typeof(int) || type == typeof(float))
            {
                return str;
            }

            return null;
        }

        static string GetPropertyName(PropertyInfo property)
        {
            string name = property.Name;

            DataMemberAttribute? dataMemberProperty = property.GetCustomAttribute<DataMemberAttribute>();
            if (dataMemberProperty != null && dataMemberProperty.Name != null)
            {
                name = dataMemberProperty.Name;
            }

            JsonPropertyNameAttribute? jsonProperty = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonProperty != null)
            {
                name = jsonProperty.Name;
            }

            return name;
        }

        static PropertyInfo? GetProperty(Type type, string key)
        {
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (GetPropertyName(property) == key)
                {
                    return property;
                }
            }

            return null;
        }

        static T SetValue<T>(PropertyInfo property, T obj, string value)
        {
            string type = property.PropertyType.Name;

            if (type == typeof(string).Name)
            {
                property.SetValue(obj, value);
            }
            else if (type == typeof(int).Name)
            {
                property.SetValue(obj, Int(value));
            }
            else if (type == typeof(float).Name)
            {
                property.SetValue(obj, Float(value));
            }
            else if (type == typeof(bool).Name)
            {
                property.SetValue(obj, Bool(value));
            }

            return obj;
        }

        static T SetValue<T>(PropertyInfo property, T obj, IEnumerable<string> values)
        {
            property.SetValue(obj, List(values));

            return obj;
        }

        public static T Parse<T>(Token[] tokens)
        {
            tokens = tokens.Where(x => x.Type != TokenType.NewLine).ToArray();

            int i = 0;

            ParserTable globalTable = new()
            {
                Key = string.Empty
            };
            List<ParserList> globalList = new();
            List<ParserTable> tables = new();

            ParserTable? table = null;
            ParserList? list = null;

            while (true)
            {
                Token token = tokens[i];

                if (token.Type == TokenType.EndOfFile) break;

                if (token.Type == TokenType.End)
                {
                    if (table != null)
                    {
                        tables.Add(table);

                        table = null;
                    }

                    if (list != null)
                    {
                        globalList.Add(list);

                        list = null;
                    }
                }

                if (token.Type == TokenType.Table)
                {
                    table = new()
                    {
                        Key = token.Value
                    };
                }

                if (token.Type == TokenType.List)
                {
                    list = new()
                    {
                        Key = token.Value
                    };
                }

                if (token.Type == TokenType.String && list != null)
                {
                    list.Values.Add(token.Value);
                }

                if (token.Type == TokenType.Equal)
                {
                    ParserValue data = new()
                    {
                        Key = tokens[i - 1].Value,
                        Value = tokens[i + 1].Value
                    };

                    if (table == null)
                    {
                        globalTable.Values.Add(data);
                    }
                    else
                    {
                        table.Values.Add(data);
                    }
                }

                i++;
            }

            T obj = Activator.CreateInstance<T>();
            if (obj == null) return obj;

            Type type = obj.GetType();

            foreach (ParserValue value in globalTable.Values)
            {
                PropertyInfo? property = GetProperty(type, value.Key);
                if (property == null) continue;

                obj = SetValue(property, obj, value.Value);
            }

            foreach (ParserList value in globalList)
            {
                PropertyInfo? property = GetProperty(type, value.Key);
                if (property == null) continue;

                obj = SetValue(property, obj, value.Values);
            }

            foreach (ParserTable vTable in tables)
            {
                PropertyInfo? propertyTable = GetProperty(type, vTable.Key);
                if (propertyTable == null) continue;

                object? objTable = Activator.CreateInstance(propertyTable.PropertyType);
                if (objTable == null) continue;

                Type typeTable = objTable.GetType();

                foreach (ParserValue value in vTable.Values)
                {
                    PropertyInfo? property = GetProperty(typeTable, value.Key);
                    if (property == null) continue;

                    objTable = SetValue(property, objTable, value.Value);
                }

                propertyTable.SetValue(obj, objTable);
            }

            return obj;
        }

        public static Token[] Unparse<T>(T obj)
        {
            if (obj == null)
                return Array.Empty<Token>();

            Dictionary<string, string> globalTable = new();
            Dictionary<string, string[]> globalList = new();
            Dictionary<string, Dictionary<string, string>> tables = new();

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                string key = GetPropertyName(property);

                object? value = property.GetValue(obj);
                if (value == null) continue;

                object[]? objList = List(value);

                if (objList != null)
                {
                    List<string> objStrList = new();

                    foreach (object vObj in objList)
                    {
                        string? vObjString = Value(vObj);

                        if (!string.IsNullOrWhiteSpace(vObjString))
                            objStrList.Add(vObjString);
                    }

                    if (objStrList.Count != 0)
                        globalList.Add(key, objStrList.ToArray());

                    continue;
                }

                string? vString = Value(value);

                if (!string.IsNullOrWhiteSpace(vString))
                {
                    globalTable.Add(key, vString);
                    continue;
                }

                Dictionary<string, string> values = new();

                foreach (PropertyInfo subProperty in value.GetType().GetProperties())
                {
                    string subKey = GetPropertyName(subProperty);

                    object? subValue = subProperty.GetValue(value);
                    if (subValue == null) continue;

                    string? subvString = Value(subValue);

                    if (!string.IsNullOrWhiteSpace(subvString))
                        values.Add(subKey, subvString);
                }

                if (values.Count != 0)
                    tables.Add(key, values);
            }

            List<Token> tokens = new();

            foreach (string key in globalTable.Keys)
            {
                string value = globalTable[key];

                tokens.AddRange(new Token[]
                {
                    new(TokenType.String, key),
                    new(TokenType.Equal),
                    new(TokenType.String, value),
                    new(TokenType.NewLine)
                });
            }

            foreach (string name in tables.Keys)
            {
                Dictionary<string, string> values = tables[name];

                tokens.AddRange(new Token[]
                    {
                        new(TokenType.NewLine),
                        new(TokenType.Table, name),
                        new(TokenType.NewLine)
                    });

                foreach (string key in values.Keys)
                {
                    string value = values[key];

                    tokens.AddRange(new Token[]
                    {
                        new(TokenType.String, key),
                        new(TokenType.Equal),
                        new(TokenType.String, value),
                        new(TokenType.NewLine)
                    });
                }

                tokens.Add(new Token(TokenType.End));
            }

            foreach (string key in globalList.Keys)
            {
                string[] values = globalList[key];

                tokens.AddRange(new Token[]
                    {
                        new(TokenType.NewLine),
                        new(TokenType.List, key),
                        new(TokenType.NewLine)
                    });

                foreach (string value in values)
                {
                    tokens.AddRange(new Token[]
                    {
                        new(TokenType.String, value),
                        new(TokenType.NewLine)
                    });
                }

                tokens.Add(new Token(TokenType.End));
            }

            tokens.Add(new Token(TokenType.EndOfFile));

            return tokens.ToArray();
        }
    }
}
