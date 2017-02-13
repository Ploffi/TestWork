using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Models.Server;

namespace WebApi.Data
{
    public class Match
    {
        public int MatchId { get; set; }

        public int TimeLimit { get; set; }
        public int FragLimit { get; set; }
        public double TimeElapsed { get; set; }

        public virtual ICollection<Score> ScoreBoard { get; set; }
        public Server Server { get; set; }
        public GameMode GameMode { get; set; }
    }
}
