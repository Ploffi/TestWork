using System;
using System.Globalization;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Services;

namespace WebApi.JsonConvertors
{
    public class MatchConvertor: JsonConverter
    {
        
           public override void  WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var match = value as Match;
            if (match == null)
                return;
            writer.WriteStartObject();

           writer.WriteProperty( "map", match.Map.Name);
           writer.WriteProperty( "gameMode", match.GameMode.Name);
           writer.WriteProperty( "fragLimit", match.FragLimit.ToString());
           writer.WriteProperty( "timeLimit", match.TimeLimit.ToString());
           writer.WriteProperty( "timeElapsed", match.TimeElapsed.ToString(CultureInfo.InvariantCulture));

            writer.WritePropertyName("scoreboard");
            writer.WriteStartArray();

            foreach (var score in match.ScoreBoard)
            {
                serializer.Serialize(writer, score);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Match);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}