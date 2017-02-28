using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Services
{

    public static class DictionaryExtensions
    {
        public static void Increment<T>(this Dictionary<T, int> dict, T key)
        {
            if (dict.ContainsKey(key))
                dict[key]++;
            else
                dict.Add(key, 1);
        }

        public static TKey KeyOfMaxValue<TKey,TValue>(this Dictionary<TKey, TValue> dict,TValue minValue) where TValue:IComparable
        {
            var max = minValue;
            TKey key = default(TKey);
            foreach (var pair in dict)
            {
                if (pair.Value.CompareTo(max) > 0)
                {
                    max = pair.Value;
                    key = pair.Key;
                }
            }
            return key;
        }

        public static Tuple<int, int> SumAndMax<TKey>(this Dictionary<TKey, int> dict)
        {
            var max = int.MinValue;
            var sum = 0;
            foreach (var pair in dict)
            {
                max = Math.Max(max, pair.Value);
                sum += pair.Value;

            }
            return Tuple.Create(sum,max);
        }
       
    }

    public static class DateTimeExtensions
    {
      
        public static DateTime Max(this DateTime time1, DateTime time2) => DateTime.Compare(time1, time2) < 0 ? time2 : time1;
        public static DateTime Min(this DateTime time1, DateTime time2) => DateTime.Compare(time1, time2) > 0 ? time2 : time1;

        public static string ToIsoFormat(this DateTime time) => time.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");

    }

    public static class WriterExtensions
    {
        public static void WriteProperty(this JsonWriter writer, string name, string property)
        {
            writer.WritePropertyName(name);
            writer.WriteValue(property);
        }
    }
}
