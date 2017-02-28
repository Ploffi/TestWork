using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Services;

namespace WebApi.JsonConvertors
{
    internal class ServerListConvertor : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var servers = value as List<Server>;

            writer.WriteStartArray();

            foreach (var server in servers)
            {
                writer.WriteStartObject();
                writer.WriteProperty("endpoint", server.EndPoint);

                writer.WritePropertyName("info");
                serializer.Serialize(writer,server);

                writer.WriteEndObject();
            }  
                  
            writer.WriteEndArray();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (List<Server>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}