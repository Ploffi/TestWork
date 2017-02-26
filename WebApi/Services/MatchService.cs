using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Controllers;
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
        private MatchPlayerContractService _MPContractService;
        private ScoreService _scoreService { get; set; }
        private MatchRepository _matchRepository { get; set; }
        private PlayerService _playerModeService { get; set; }
        private ServerService _serverService { get; set; }
        public MatchService()
        {
            _scoreService = new ScoreService();
            _matchRepository = new MatchRepository();
            _playerModeService = new PlayerService();
            _serverService = new ServerService();
            _scoresRepository = new ScoreRepository();
            _playerService = new PlayerService();
            _mapService = new MapService();
            _MPContractService = new MatchPlayerContractService();
        }

        
        public void FillDataBase(int count)
        {
            var server = new Server()
            {
                EndPoint = "endpoint",
                Name = "name"
            };
            var idx = 0;
            var gameModesCollection = new GameMode[3] {new GameMode(), new GameMode(), new GameMode() };
            var col = Enumerable.Range(1, count).Select(id => new Match()
            {
                FragLimit = 2,
                GameMode = gameModesCollection[idx++ % 3],
                Server = server,
                TimeElapsed = 2.6,
                TimeLimit = 30
            }).ToList();
            var path = ConfigurationManager.AppSettings["dbPath"];
            var stope = new Stopwatch();
            stope.Start();
            using (var db = new LiteDatabase(path))
            {
                var servers = db.GetCollection<Server>("servers");
                var gameModes = db.GetCollection<GameMode>("game_modes");
                var matches = db.GetCollection<Match>("matches");

               ;
                Console.WriteLine(stope.ElapsedMilliseconds);
                foreach (var se in servers.Include(x => x.EndPoint).FindAll())
                {
                    Console.WriteLine(se.EndPoint);

                }
               
            }
            stope.Stop();
        }

        public bool TryInsert(MatchModel model)
        {
            var gameMode = model.Server.GameModes.FirstOrDefault(mode => mode.Name == model.GameMode);
            var map = _mapService.GetOrInsertByName(model.Map);
            if ( gameMode == null)
                return false;
            var match = new Match
            {
                ServerId = model.Server.ServerId,
                Server = model.Server,
                FragLimit = model.FragLimit,
                GameMode = gameMode,
                Date = model.Date,
                Map = map,
                TimeElapsed = model.TimeElapsed,
                TimeLimit = model.TimeLimit
            };

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
