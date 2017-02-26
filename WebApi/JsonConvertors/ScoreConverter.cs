using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.Data;

namespace WebApi.JsonConvertors
{
    class ScoreConverter : Convertor
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var score = value as Score;
            if (score == null)
                 return;

            writer.WriteStartObject();
            writer.WriteProperty("name",score.Player.Name);
            writer.WriteProperty("frags",score.Frags.ToString());
            writer.WriteProperty("kills",score.Kills.ToString());
            writer.WriteProperty("deaths",score.Deaths.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Score);
        }
    }
}
