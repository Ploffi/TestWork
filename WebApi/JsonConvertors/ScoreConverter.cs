﻿using System;
using Newtonsoft.Json;
using WebApi.Data;
using WebApi.Services;

namespace WebApi.JsonConvertors
{
    public class ScoreConverter :JsonConverter
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
