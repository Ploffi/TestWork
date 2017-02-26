using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Data;
using WebApi.Repository;

namespace WebApi.Models
{
    public class ServerModel
    {
        public int ServerId { get; set; }
        public string Name { get; set; }
        public string EndPoint { get; set; }

        public string[] GameModes { get; set; }

        public ServerModel(string name, string[] gameModes)
        {
            Name = name;
            GameModes = gameModes;
        }    
    }
}
