using System;
using System.Globalization;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Services;

namespace WebApi.JsonConvertors
{
    public class PlayerConverter:JsonConverter
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
