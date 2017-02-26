using System.Collections.Generic;
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
    }
}
