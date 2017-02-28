using System;
using System.Linq;
using WebApi.Data;

namespace WebApi.Models
{
    public class MatchModel
    {
        public int? MatchId { get; set; }
        public int? TimeLimit { get; set; }
        public int? FragLimit { get; set; }
        public double? TimeElapsed { get; set; }
        public string Map { get; set; }
        public DateTime? Date { get; set; }    
        public ScoreModel[] ScoreBoard { get; set; }
        public string GameMode { get; set; }
        public string ServerEndPoint { get; set; }
        public Server Server { get; set; }

        public MatchModel()
        {
            
        }
        public MatchModel(string map,string gameMode,int fragLimit,int timeLimit,double timeElapsed,ScoreModel[] scoreboard)
        {
            Map = map;
            FragLimit = fragLimit;
            TimeLimit = timeLimit;
            TimeElapsed = timeElapsed;
            ScoreBoard = scoreboard;
            GameMode = gameMode;
        }

        public bool IsNotValid()
        {
            return string.IsNullOrEmpty(Map) || string.IsNullOrEmpty(GameMode) ||
                   !FragLimit.HasValue || !TimeLimit.HasValue || !TimeElapsed.HasValue
                   || ScoreBoard.Any(sc => sc.IsNotValid());

        }
    }
}
