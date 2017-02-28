using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ServerStats
    {
        public int totalMatchesPlayed;
        public int maximumMatchesPerDay;
        public double averageMatchesPerDay;
        public int maximumPopulation;
        public double averagePopulation;
        public string[] top5GameModes = new string[0];
        public string[] top5Maps = new string[0];

    }
}
