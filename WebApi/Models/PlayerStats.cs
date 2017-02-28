using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class PlayerStats
    {
        public int totalMatchesPlayed;
        public int totalMatchesWon;
        public string favoriteServer;
        public int uniqueServers;
        public string favoriteGameMode;
        public double averageScoreboardPercent;
        public int maximumMatchesPerDay;
        public double averageMatchesPerDay;
        public string lastMatchPlayed;
        public double killToDeathRatio;

    }
}
