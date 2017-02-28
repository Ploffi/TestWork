using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Services;

namespace WebApi.JsonConvertors
{
    public class PlayerConverter:Convertor
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var player = value as Player;

            writer.WriteStartObject();
            writer.WriteProperty("name",player.Name);
            writer.WriteProperty("killToDeathRatio",player.KillsToDeathRatio.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Player);
        }
    }
}
