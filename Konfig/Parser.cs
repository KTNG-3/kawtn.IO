using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace kawtn.IO.Konfig
{
    class Info
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Info(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }

    class Table
    {
        public string Key { get; set; }
        public List<Info> Values { get; set; }

        public Table(string key)
        {
            this.Key = key;
            this.Values = new List<Info>();
        }
    }

    class Collection
    {
        public string Key { get; set; }
        public List<string> Values { get; set; }

        public Collection(string key)
        {
            this.Key = key;
            this.Values = new List<string>();
        }
    }

    static class Parser
    {
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

            if (type == typeof(bool) || type.IsEnum)
            {
                return str.ToLower();
            }

            if (type == typeof(byte) ||
                type == typeof(char) ||
                type == typeof(DateTime) ||
                type == typeof(decimal) ||
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(short))
            {
                return str;
            }

            return null;
        }

        static MemberInfo[] GetMember(Type type)
        {
            List<MemberInfo> members = new();

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.IsStatic) continue;

                members.Add(field);
            }

            foreach (PropertyInfo property in type.GetProperties(~BindingFlags.Static))
            {
                if (property.GetIndexParameters().Length != 0) continue; // indexer

                members.Add(property);
            }

            return members.ToArray();
        }

        static MemberInfo? GetMember(Type type, string key)
        {
            foreach (MemberInfo member in Parser.GetMember(type))
            {
                if (Parser.GetMemberName(member) == key)
                {
                    return member;
                }
            }

            return null;
        }

        static string GetMemberName(MemberInfo member)
        {
            DataMemberAttribute? dataMember = member.GetCustomAttribute<DataMemberAttribute>();
            if (dataMember != null && !string.IsNullOrWhiteSpace(dataMember.Name))
            {
                return dataMember.Name;
            }

            JsonPropertyNameAttribute? jsonProperty = member.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonProperty != null)
            {
                return jsonProperty.Name;
            }

            return member.Name;
        }

        static object? GetMemberValue(MemberInfo member, object obj)
        {
            if (member is PropertyInfo property)
            {
                if (!property.CanRead)
                {
                    return null;
                }

                return property.GetValue(obj);
            }

            if (member is FieldInfo field)
            {
                return field.GetValue(obj);
            }

            return null;
        }

        static Type? GetMemberValueType(MemberInfo member)
        {
            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            if (member is FieldInfo field)
            {
                return field.FieldType;
            }

            return null;
        }

        static void SetMemberValue<T>(MemberInfo member, T obj, object value)
        {
            if (value == null || Parser.GetMemberValueType(member) != value.GetType()) return;

            if (member is PropertyInfo property)
            {
                if (!property.CanWrite) return;

                property.SetValue(obj, value);
            }

            if (member is FieldInfo field)
            {
                field.SetValue(obj, value);
            }
        }

        static void SetValue<T>(MemberInfo member, T obj, string value)
        {
            Type? type = Parser.GetMemberValueType(member);
            if (type == null) return;

            object? content = TypeConversion.Convert(type, value);
            if (content == null) return;

            Parser.SetMemberValue(member, obj, content);
        }

        static void SetValue<T>(MemberInfo member, T obj, IEnumerable<string> values)
        {
            Type? type = Parser.GetMemberValueType(member);
            if (type == null) return;

            object? content = TypeConversion.Convert(type, values);
            if (content == null) return;

            Parser.SetMemberValue(member, obj, content);
        }

        static T CreateInstance<T>()
        {
            try
            {
                return Activator.CreateInstance<T>();
            }
            catch { }

            return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
        }

        static object CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch { }

            return RuntimeHelpers.GetUninitializedObject(type);
        }

        public static T Parse<T>(Token[] tokens)
        {
            tokens = tokens.Where(x => x.Type != TokenType.NewLine).ToArray();

            int i = 0;

            Table globalTable = new(string.Empty);
            List<Collection> globalList = new();
            List<Table> tables = new();

            Table? table = null;
            Collection? list = null;

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
                    table = new Table(token.Value);
                }

                if (token.Type == TokenType.List)
                {
                    list = new Collection(token.Value);
                }

                if (token.Type == TokenType.String && list != null)
                {
                    list.Values.Add(token.Value);
                }

                if (token.Type == TokenType.Equal)
                {
                    Info data = new(
                        key: tokens[i - 1].Value,
                        value: tokens[i + 1].Value
                        );

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

            T obj = Parser.CreateInstance<T>();
            if (obj == null)
            {
                return obj;
            }

            Type type = obj.GetType();

            foreach (Info value in globalTable.Values)
            {
                MemberInfo? member = Parser.GetMember(type, value.Key);
                if (member == null) continue;

                Parser.SetValue(member, obj, value.Value);
            }

            foreach (Collection value in globalList)
            {
                MemberInfo? member = Parser.GetMember(type, value.Key);
                if (member == null) continue;

                Parser.SetValue(member, obj, value.Values);
            }

            foreach (Table vTable in tables)
            {
                MemberInfo? memberTable = Parser.GetMember(type, vTable.Key);
                if (memberTable == null) continue;

                Type? memberValueType = Parser.GetMemberValueType(memberTable);
                if (memberValueType == null) continue;

                object? objTable = Parser.CreateInstance(memberValueType);
                if (objTable == null) continue;

                Type typeTable = objTable.GetType();

                foreach (Info value in vTable.Values)
                {
                    MemberInfo? member = Parser.GetMember(typeTable, value.Key);
                    if (member == null) continue;

                    Parser.SetValue(member, objTable, value.Value);
                }

                Parser.SetMemberValue(memberTable, obj, objTable);
            }

            return obj;
        }

        public static Token[] Unparse<T>(T obj)
        {
            if (obj == null)
            {
                return Array.Empty<Token>();
            }

            Dictionary<string, string> globalTable = new();
            Dictionary<string, string[]> globalList = new();
            Dictionary<string, Dictionary<string, string>> tables = new();

            foreach (MemberInfo member in Parser.GetMember(obj.GetType()))
            {
                string key = Parser.GetMemberName(member);

                object? value = Parser.GetMemberValue(member, obj);
                if (value == null) continue;

                IEnumerable<object>? objList = TypeConversion.ToObjectCollection(value);

                if (objList != null)
                {
                    List<string> objStrList = new();

                    foreach (object vObj in objList)
                    {
                        string? vObjString = Parser.Value(vObj);

                        if (!string.IsNullOrWhiteSpace(vObjString))
                        {
                            objStrList.Add(vObjString);
                        }
                    }

                    if (objStrList.Count != 0)
                    {
                        globalList.Add(key, objStrList.ToArray());
                    }

                    continue;
                }

                string? vString = Parser.Value(value);

                if (vString != null)
                {
                    if (string.IsNullOrWhiteSpace(vString))
                    {
                        continue;
                    }

                    globalTable.Add(key, vString);
                    continue;
                }

                Dictionary<string, string> values = new();

                foreach (MemberInfo subMember in Parser.GetMember(value.GetType()))
                {
                    string subKey = Parser.GetMemberName(subMember);

                    object? subValue = Parser.GetMemberValue(subMember, value);
                    if (subValue == null) continue;

                    string? subvString = Parser.Value(subValue);

                    if (!string.IsNullOrWhiteSpace(subvString))
                    {
                        values.Add(subKey, subvString);
                    }
                }

                if (values.Count != 0)
                {
                    tables.Add(key, values);
                }
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
