using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace WebApi.Data
{
    public class Server
    {
        public int ServerId { get; set; }
        public string Name { get; set; }

        [BsonIndex(true)]
        public string EndPoint { get; set; }

        [BsonRef("game_modes")]
        public List<GameMode> GameModes { get; set; }

        public override bool Equals(object obj)
        {
            var server = obj as Server;
            if (server == null)
                return false;
            return server.Name == Name && GameModes.All(mode => server.GameModes.Contains(mode));
        }
    }
}
