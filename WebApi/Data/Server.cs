using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WebApi.Data
{
    public class Server
    {
        public int ServerId { get; set; }
        public string Name { get; set; }

        [BsonId(true)]
       
        public string EndPoint { get; set; }

        [BsonRef("game_modes")]
        public List<GameMode> GameModes { get; set; }

        public Server()
        { }
        public Server(string name, GameMode[] gameModes)
        {
            Name = name;
            GameModes = gameModes.ToList();
        }
    }

    public class ServerModel
    {
        public int ServerId { get; set; }
        public string Name { get; set; }
        public string EndPoint { get; set; }

        public IEnumerable<string> GameModes { get; set; }

        public ServerModel(string name, IEnumerable<string> gameModes)
        {
            Name = name;
            GameModes = gameModes;
        }

    }
}
