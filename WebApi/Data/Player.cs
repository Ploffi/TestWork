using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WebApi.Data
{
    public class Player
    {
        public int PlayerId { get; set; }
        [BsonIndex(true)]
        public string Name { get; set; }

        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }

        [BsonIndex]
        public double KillsToDeathRatio { get; set; }

        [BsonIgnore]
        public List<Score> Scores { get; set; }
        [BsonIgnore]
        public List<Match> Matches { get; set; }

        public Player() { }
    }

}
