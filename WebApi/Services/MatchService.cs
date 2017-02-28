using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using LiteDB;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Services
{
    class MatchService
    {
        private ScoreRepository _scoresRepository;
        private PlayerService _playerService;
        private MapService _mapService;
        private UtilRepository _utilRepository;
        private ScoreService _scoreService;
        private MatchRepository _matchRepository;
        public MatchService()
        {
            _scoreService = new ScoreService();
            _matchRepository = new MatchRepository();
            _scoresRepository = new ScoreRepository();
            _playerService = new PlayerService();
            _mapService = new MapService();
            _utilRepository = new UtilRepository();
        }  
        public bool TryInsert(MatchModel model)
        {
            var gameMode = model.Server.GameModes.FirstOrDefault(mode => mode.Name == model.GameMode);
          
            if ( gameMode == null)
                return false;

            var map = _mapService.GetOrInsertByName(model.Map);
            var match = new Match
            {
                ServerId = model.Server.ServerId,
                Server = model.Server,
                FragLimit = model.FragLimit.Value,
                GameMode = gameMode,
                Date = model.Date.Value,
                Map = map,
                TimeElapsed = model.TimeElapsed.Value,
                TimeLimit = model.TimeLimit.Value
            };
            _utilRepository.UpdateMaxDate(match.Date);
            _matchRepository.Insert(match);

            var players = _playerService.UpsertByScore(model.ScoreBoard).ToList();
           
            var scores = _scoreService.InsertOrderedScores(model.ScoreBoard,players,match.MatchId);

            match.ScoreBoard = scores;

            _matchRepository.Update(match);

            return true;
        }

        public Match GetMatchByTimeStampOnServer(string endPoint, DateTime time)
        {
            var match = _matchRepository.GetMatchByTimeStampOnServer(endPoint, time);
            if (match == null)
                return null;

            var idsAsBsonArray = match.ScoreBoard.Select(sc => new BsonValue(sc.ScoreId));

            match.ScoreBoard = _scoresRepository.
                GetScoreBoardByIds(idsAsBsonArray)
                .ToList();
            return match;
        }
    }
}
