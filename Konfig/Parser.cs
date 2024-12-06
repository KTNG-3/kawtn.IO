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
                type == typeof(short) ||
                type == typeof(string))
            {
                return str;
            }

            return null;
        }

        static MemberInfo[] GetMember(Type type)
        {
            List<MemberInfo> members = new List<MemberInfo>();

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

        static string GetMemberName(MemberInfo member)
        {
            string name = member.Name;

            DataMemberAttribute? dataMember = member.GetCustomAttribute<DataMemberAttribute>();
            if (dataMember != null && !string.IsNullOrWhiteSpace(dataMember.Name))
            {
                name = dataMember.Name;
            }

            JsonPropertyNameAttribute? jsonProperty = member.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonProperty != null)
            {
                name = jsonProperty.Name;
            }

            return name;
        }


        static MemberInfo? GetMember(Type type, string key)
        {
            foreach (MemberInfo member in GetMember(type))
            {
                if (GetMemberName(member) == key)
                {
                    return member;
                }
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

        static object? GetMemberValue(MemberInfo member, object obj)
        {
            if (member is PropertyInfo property)
            {
                if (!property.CanRead)
                    return null;

                return property.GetValue(obj);
            }

            if (member is FieldInfo field)
            {
                return field.GetValue(obj);
            }

            return null;
        }

        static void SetMemberValue<T>(MemberInfo member, T obj, object value)
        {
            if (value == null || GetMemberValueType(member) != value.GetType())
                return;

            if (member is PropertyInfo property)
            {
                if (!property.CanWrite)
                    return;

                property.SetValue(obj, value);
            }

            if (member is FieldInfo field)
            {
                field.SetValue(obj, value);
            }
        }

        static void SetValue<T>(MemberInfo member, T obj, string value)
        {
            Type? type = GetMemberValueType(member);
            if (type == null) return;

            object? content = TypeConversion.Convert(type, value);
            if (content == null) return;

            SetMemberValue(member, obj, content);
        }

        static void SetValue<T>(MemberInfo member, T obj, IEnumerable<string> values)
        {
            Type? type = GetMemberValueType(member);
            if (type == null) return;

            object? content = TypeConversion.Convert(type, values);
            if (content == null) return;

            SetMemberValue(member, obj, content);
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

            Table globalTable = new Table(string.Empty);
            List<Collection> globalList = new List<Collection>();
            List<Table> tables = new List<Table>();

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
                    Info data = new Info(
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

            T obj = CreateInstance<T>();
            if (obj == null) return obj;

            Type type = obj.GetType();

            foreach (Info value in globalTable.Values)
            {
                MemberInfo? member = GetMember(type, value.Key);
                if (member == null) continue;

                SetValue(member, obj, value.Value);
            }

            foreach (Collection value in globalList)
            {
                MemberInfo? member = GetMember(type, value.Key);
                if (member == null) continue;

                SetValue(member, obj, value.Values);
            }

            foreach (Table vTable in tables)
            {
                MemberInfo? memberTable = GetMember(type, vTable.Key);
                if (memberTable == null) continue;

                Type? memberValueType = GetMemberValueType(memberTable);
                if (memberValueType == null) continue;

                object? objTable = CreateInstance(memberValueType);
                if (objTable == null) continue;

                Type typeTable = objTable.GetType();

                foreach (Info value in vTable.Values)
                {
                    MemberInfo? member = GetMember(typeTable, value.Key);
                    if (member == null) continue;

                    SetValue(member, objTable, value.Value);
                }

                SetMemberValue(memberTable, obj, objTable);
            }

            return obj;
        }

        public static Token[] Unparse<T>(T obj)
        {
            if (obj == null)
                return Array.Empty<Token>();

            Dictionary<string, string> globalTable = new Dictionary<string, string>();
            Dictionary<string, string[]> globalList = new Dictionary<string, string[]>();
            Dictionary<string, Dictionary<string, string>> tables = new Dictionary<string, Dictionary<string, string>>();

            foreach (MemberInfo member in GetMember(obj.GetType()))
            {
                string key = GetMemberName(member);

                object? value = GetMemberValue(member, obj);
                if (value == null) continue;

                IEnumerable<object>? objList = TypeConversion.ToObjectCollection(value);

                if (objList != null)
                {
                    List<string> objStrList = new List<string>();

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

                Dictionary<string, string> values = new Dictionary<string, string>();

                foreach (MemberInfo subMember in GetMember(value.GetType()))
                {
                    string subKey = GetMemberName(subMember);

                    object? subValue = GetMemberValue(subMember, value);
                    if (subValue == null) continue;

                    string? subvString = Value(subValue);

                    if (!string.IsNullOrWhiteSpace(subvString))
                        values.Add(subKey, subvString);
                }

                if (values.Count != 0)
                    tables.Add(key, values);
            }

            List<Token> tokens = new List<Token>();

            foreach (string key in globalTable.Keys)
            {
                string value = globalTable[key];

                tokens.AddRange(new Token[]
                {
                    new Token(TokenType.String, key),
                    new Token(TokenType.Equal),
                    new Token(TokenType.String, value),
                    new Token(TokenType.NewLine)
                });
            }

            foreach (string name in tables.Keys)
            {
                Dictionary<string, string> values = tables[name];

                tokens.AddRange(new Token[]
                    {
                        new Token(TokenType.NewLine),
                        new Token(TokenType.Table, name),
                        new Token(TokenType.NewLine)
                    });

                foreach (string key in values.Keys)
                {
                    string value = values[key];

                    tokens.AddRange(new Token[]
                    {
                        new Token(TokenType.String, key),
                        new Token(TokenType.Equal),
                        new Token(TokenType.String, value),
                        new Token(TokenType.NewLine)
                    });
                }

                tokens.Add(new Token(TokenType.End));
            }

            foreach (string key in globalList.Keys)
            {
                string[] values = globalList[key];

                tokens.AddRange(new Token[]
                    {
                        new Token(TokenType.NewLine),
                        new Token(TokenType.List, key),
                        new Token(TokenType.NewLine)
                    });

                foreach (string value in values)
                {
                    tokens.AddRange(new Token[]
                    {
                        new Token(TokenType.String, value),
                        new Token(TokenType.NewLine)
                    });
                }

                tokens.Add(new Token(TokenType.End));
            }

            tokens.Add(new Token(TokenType.EndOfFile));

            return tokens.ToArray();
        }
    }
}
