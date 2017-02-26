using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.JsonConvertors
{
    abstract class Convertor : JsonConverter
    {
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
