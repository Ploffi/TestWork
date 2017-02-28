using System.Collections.Generic;
using System.Linq;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Services
{
    public class ScoreService
    {
        private ScoreRepository _scoreRepository;
        private PlayerService _playerService;

        public ScoreService()
        {
            _scoreRepository = new ScoreRepository();
            _playerService = new PlayerService();
        }

        public List<Score> InsertOrderedScores(ScoreModel[] scoreBoard,List<Player> players,int matchId)
        {
            var idx = 1;         
            var scoresWithPosition = scoreBoard.Select(model =>
                   new Score()
                {
                    Player = players[idx - 1],
                    Deaths = model.Deaths.Value,
                    Frags = model.Frags.Value,
                    Kills = model.Kills.Value,
                    Position = idx++,
                    MatchId = matchId,
                    
                }
            ).ToList();
            _scoreRepository.Insert(scoresWithPosition);
            return scoresWithPosition;
        }
    }
}