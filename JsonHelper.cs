using System;
using Newtonsoft.Json;

namespace EnglishWordsPrintUtility
{
    public static class JsonExtensions
    {
        public static string SerialiseToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static object DeserialiseToObject(this string serialized, Type type)
        {
            return JsonConvert.DeserializeObject(serialized, type);
        }
    }
}
