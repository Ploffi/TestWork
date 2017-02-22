using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Models;

namespace WebApi.Data
{
    public class Match
    {
        public int MatchId { get; set; }
        public int TimeLimit { get; set; }
        public int FragLimit { get; set; }
        public double TimeElapsed { get; set; }
        public DateTime Date { get; set; }

        [BsonRef("scores")]
        public List<Score> ScoreBoard { get; set; }
        [BsonRef("scores")]
        public Server Server { get; set; }
        [BsonRef("game_modes")]
        public  GameMode GameMode { get; set; }
    }
}
