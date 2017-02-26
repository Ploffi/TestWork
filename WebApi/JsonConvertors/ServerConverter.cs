using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WebApi.Controllers;
using WebApi.Data;

namespace WebApi.JsonConvertors
{
    class ServerConvertor : Convertor
    {
        private bool onlyServerInfo;

        public ServerConvertor(bool onlyServerInfo = true)
        {
            this.onlyServerInfo = onlyServerInfo;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var server = value as Server;
            if (server == null)
                return;
            writer.WriteStartObject();

            if (onlyServerInfo)
            {
                WriteServerInfo(writer,serializer,server);
                writer.WriteEndObject();
                return;
            }

            writer. WriteProperty("endPoint", server.EndPoint);

            writer.WritePropertyName("info");
            writer.WriteStartObject();

            WriteServerInfo(writer, serializer, server);

            writer.WriteEndObject();

            writer.WriteEndObject();


        }

        private void WriteServerInfo(JsonWriter writer, JsonSerializer serializer, Server server)
        {
            writer. WriteProperty("name", server.Name);
            writer.WritePropertyName("gameModes");
            writer.WriteStartArray();
            foreach (var mode in server.GameModes)
            {
                serializer.Serialize(writer, mode.Name);
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (Server);
        }
    }
}
