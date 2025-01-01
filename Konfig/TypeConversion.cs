using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace kawtn.IO.Konfig
{
    static class TypeConversion
    {
        public static bool ToBoolean(string value)
        {
            return System.Convert.ToBoolean(value);
        }

        public static byte ToByte(string value)
        {
            return System.Convert.ToByte(value);
        }

        public static char ToChar(string value)
        {
            return System.Convert.ToChar(value);
        }

        public static DateTime ToDateTime(string value)
        {
            return System.Convert.ToDateTime(value);
        }

        public static decimal ToDecimal(string value)
        {
            return System.Convert.ToDecimal(value);
        }

        public static double ToDouble(string value)
        {
            return System.Convert.ToDouble(value);
        }

        public static float ToFloat(string value)
        {
            return System.Convert.ToSingle(value);
        }

        public static int ToInt(string value)
        {
            return System.Convert.ToInt32(value);
        }

        public static long ToLong(string value)
        {
            return System.Convert.ToInt64(value);
        }

        public static short ToShort(string value)
        {
            return System.Convert.ToInt16(value);
        }

        public static string ToString(string value)
        {
            return System.Convert.ToString(value);
        }

        public static object ToEnum(Type type, string value)
        {
            return Enum.Parse(type, value, ignoreCase: true);
        }

        public static IEnumerable<bool> ToBoolean(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToBoolean);
        }

        public static IEnumerable<byte> ToByte(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToByte);
        }

        public static IEnumerable<char> ToChar(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToChar);
        }

        public static IEnumerable<DateTime> ToDateTime(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToDateTime);
        }

        public static IEnumerable<decimal> ToDecimal(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToDecimal);
        }

        public static IEnumerable<double> ToDouble(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToDouble);
        }

        public static IEnumerable<float> ToFloat(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToFloat);
        }

        public static IEnumerable<int> ToInt(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToInt);
        }

        public static IEnumerable<long> ToLong(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToLong);
        }

        public static IEnumerable<short> ToShort(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToShort);
        }

        public static IEnumerable<string> ToString(IEnumerable<string> values)
        {
            return values.Select(TypeConversion.ToString);
        }

        public static IEnumerable<object> ToEnum(Type enumType, IEnumerable<string> values)
        {
            return values.Select(x => TypeConversion.ToEnum(enumType, x));
        }

        public static T[] ToArray<T>(IEnumerable<T> values)
        {
            return values.ToArray();
        }

        public static HashSet<T> ToHashSet<T>(IEnumerable<T> values)
        {
            return values.ToHashSet();
        }

        public static List<T> ToList<T>(IEnumerable<T> values)
        {
            return values.ToList();
        }

        public static Queue<T> ToQueue<T>(IEnumerable<T> values)
        {
            return new Queue<T>(values);
        }

        public static Stack<T> ToStack<T>(IEnumerable<T> values)
        {
            return new Stack<T>(values);
        }

        public static IEnumerable<object>? ToObjectCollection(object value)
        {
            if (value is ICollection enumerable)
            {
                return enumerable.Cast<object>();
            }

            return null;
        }

        public static object? Convert(Type type, string value)
        {
            if (type == typeof(bool))
                return TypeConversion.ToBoolean(value);

            if (type == typeof(byte))
                return TypeConversion.ToByte(value);

            if (type == typeof(char))
                return TypeConversion.ToChar(value);

            if (type == typeof(DateTime))
                return TypeConversion.ToDateTime(value);

            if (type == typeof(decimal))
                return TypeConversion.ToDecimal(value);

            if (type == typeof(double))
                return TypeConversion.ToDouble(value);

            if (type == typeof(float))
                return TypeConversion.ToFloat(value);

            if (type == typeof(int))
                return TypeConversion.ToInt(value);

            if (type == typeof(long))
                return TypeConversion.ToLong(value);

            if (type == typeof(short))
                return TypeConversion.ToShort(value);

            if (type == typeof(string))
                return TypeConversion.ToString(value);

            if (type.IsEnum)
                return TypeConversion.ToEnum(type, value);

            return null;
        }

        static object? ConvertCollection<TValue>(Type collectionType, IEnumerable<TValue> values)
        {
            if (collectionType.IsArray)
                return TypeConversion.ToArray(values);

            if (!collectionType.IsGenericType)
            {
                return null;
            }    

            Type typeDefinition = collectionType.GetGenericTypeDefinition();

            if (typeDefinition == typeof(HashSet<>))
                return TypeConversion.ToHashSet(values);

            if (typeDefinition == typeof(List<>))
                return TypeConversion.ToList(values);

            if (typeDefinition == typeof(Queue<>))
                return TypeConversion.ToQueue(values);

            if (typeDefinition == typeof(Stack<>))
                return TypeConversion.ToStack(values);

            return null;
        }

        public static object? Convert(Type type, IEnumerable<string> values)
        {
            Type elementType = type.IsGenericType
                ? type.GetGenericArguments().Single()
                : type.GetElementType();

            if (elementType == typeof(bool))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToBoolean(values));

            if (elementType == typeof(byte))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToByte(values));

            if (elementType == typeof(char))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToChar(values));

            if (elementType == typeof(DateTime))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToDateTime(values));

            if (elementType == typeof(decimal))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToDecimal(values));

            if (elementType == typeof(double))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToDouble(values));

            if (elementType == typeof(float))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToFloat(values));

            if (elementType == typeof(int))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToInt(values));

            if (elementType == typeof(long))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToLong(values));

            if (elementType == typeof(short))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToShort(values));

            if (elementType == typeof(string))
                return TypeConversion.ConvertCollection(type, TypeConversion.ToString(values));

            if (elementType.IsEnum)
                return TypeConversion.ConvertCollection(type, TypeConversion.ToEnum(elementType, values));

            return null;
        }
    }
}
