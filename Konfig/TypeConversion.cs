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
            return values.Select(ToBoolean);
        }

        public static IEnumerable<byte> ToByte(IEnumerable<string> values)
        {
            return values.Select(ToByte);
        }

        public static IEnumerable<char> ToChar(IEnumerable<string> values)
        {
            return values.Select(ToChar);
        }

        public static IEnumerable<DateTime> ToDateTime(IEnumerable<string> values)
        {
            return values.Select(ToDateTime);
        }

        public static IEnumerable<decimal> ToDecimal(IEnumerable<string> values)
        {
            return values.Select(ToDecimal);
        }

        public static IEnumerable<double> ToDouble(IEnumerable<string> values)
        {
            return values.Select(ToDouble);
        }

        public static IEnumerable<float> ToFloat(IEnumerable<string> values)
        {
            return values.Select(ToFloat);
        }

        public static IEnumerable<int> ToInt(IEnumerable<string> values)
        {
            return values.Select(ToInt);
        }

        public static IEnumerable<long> ToLong(IEnumerable<string> values)
        {
            return values.Select(ToLong);
        }

        public static IEnumerable<short> ToShort(IEnumerable<string> values)
        {
            return values.Select(ToShort);
        }

        public static IEnumerable<string> ToString(IEnumerable<string> values)
        {
            return values.Select(ToString);
        }

        public static IEnumerable<object> ToEnum(Type enumType, IEnumerable<string> values)
        {
            return values.Select(x => ToEnum(enumType, x));
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
            if (value is IEnumerable enumerable)
            {
                return enumerable.Cast<object>();
            }

            return null;
        }

        public static object? Convert(Type type, string value)
        {
            if (type == typeof(bool)) return ToBoolean(value);
            if (type == typeof(byte)) return ToByte(value);
            if (type == typeof(char)) return ToChar(value);
            if (type == typeof(DateTime)) return ToDateTime(value);
            if (type == typeof(decimal)) return ToDecimal(value);
            if (type == typeof(double)) return ToDouble(value);
            if (type == typeof(float)) return ToFloat(value);
            if (type == typeof(int)) return ToInt(value);
            if (type == typeof(long)) return ToLong(value);
            if (type == typeof(short)) return ToShort(value);
            if (type == typeof(string)) return ToString(value);
            if (type.IsEnum) return ToEnum(type, value);

            return null;
        }

        static object? ConvertCollection<TValue>(Type collectionType, IEnumerable<TValue> values)
        {
            if (collectionType.IsArray)
                return ToArray(values);

            if (!collectionType.IsGenericType)
                return null;

            Type typeDefinition = collectionType.GetGenericTypeDefinition();

            if (typeDefinition == typeof(HashSet<>))
                return ToHashSet(values);

            if (typeDefinition == typeof(List<>))
                return ToList(values);

            if (typeDefinition == typeof(Queue<>))
                return ToQueue(values);

            if (typeDefinition == typeof(Stack<>))
                return ToStack(values);

            return null;
        }

        public static object? Convert(Type type, IEnumerable<string> values)
        {
            Type elementType = type.IsGenericType
                ? type.GetGenericArguments().Single()
                : type.GetElementType();

            if (elementType == typeof(bool)) return ConvertCollection(type, ToBoolean(values));
            if (elementType == typeof(byte)) return ConvertCollection(type, ToByte(values));
            if (elementType == typeof(char)) return ConvertCollection(type, ToChar(values));
            if (elementType == typeof(DateTime)) return ConvertCollection(type, ToDateTime(values));
            if (elementType == typeof(decimal)) return ConvertCollection(type, ToDecimal(values));
            if (elementType == typeof(double)) return ConvertCollection(type, ToDouble(values));
            if (elementType == typeof(float)) return ConvertCollection(type, ToFloat(values));
            if (elementType == typeof(int)) return ConvertCollection(type, ToInt(values));
            if (elementType == typeof(long)) return ConvertCollection(type, ToLong(values));
            if (elementType == typeof(short)) return ConvertCollection(type, ToShort(values));
            if (elementType == typeof(string)) return ConvertCollection(type, ToString(values));
            if (elementType.IsEnum) return ConvertCollection(type, ToEnum(elementType, values));

            return null;
        }
    }
}
