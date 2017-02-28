using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebApi.Data;
using WebApi.Services;

namespace WebApi.JsonConvertors
{
   public  class RecentMatchesConvertor:JsonConverter
    {
        public override void  WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var matches = value as List<Match>;
            if (matches == null)
            {
                return;
            }

            writer.WriteStartArray();
            foreach (var match in matches)
            {
                writer.WriteStartObject();

                writer.WriteProperty("server",match.Server.EndPoint);
                writer.WriteProperty("timestamp",match.Date.ToIsoFormat());

                writer.WritePropertyName("results");
                serializer.Serialize(writer,match);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (List<Match>);
        }
    }
}
