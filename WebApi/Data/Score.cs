using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WebApi.Data
{
    public class Score
    {
        public int ScoreId { get; set; }

        [BsonRef("players")]
        public Player Player { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }

        public string Name => Player.Name;
    }
}
