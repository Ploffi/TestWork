using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WebApi.Data
{
    public class GameMode
    {
        public int GameModeId { get; set; }
        [BsonIndex(true)]
        public string Name { get; set; }
    }
}
