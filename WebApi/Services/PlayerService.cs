﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LiteDB;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Services
{
    internal class PlayerService
    {
        private PlayerRepository _playerRepository;
        private ScoreRepository _scoreRepository ;
        private MatchRepository _matchRepository;

        public PlayerService()
        {
            _playerRepository = new PlayerRepository();
            _scoreRepository = new ScoreRepository();
            _matchRepository = new MatchRepository();
        }

        public Player GetByName(string playerName)
        {    
           return _playerRepository.GetByName(playerName);
        }

        public IEnumerable<Player> UpsertByScore(IEnumerable<ScoreModel> scores, bool withoutJournal = false)
        {    
            Func<ScoreModel,Player> creator = score => new Player()
            {
                TotalKills = score.Kills,
                TotalDeaths = score.Deaths,
                KillsToDeathRatio = (double)score.Kills / Math.Max(1,score.Deaths)
            };

            Action<ScoreModel, Player> updater = (score, player) =>
            {
                player.TotalKills += score.Kills;
                player.TotalDeaths += score.Deaths;
                player.KillsToDeathRatio = (double) player.TotalKills/Math.Max(1, player.TotalDeaths);
            };


            return _playerRepository.GetOrInsertByScoreModel(scores,creator,updater, withoutJournal);
           
        }

        public object GetPlayerStats(string playerName)
        {
            var player = _playerRepository.GetByName(playerName);
            var allScores = _scoreRepository.GetScoreBoardByPlayerId(player.PlayerId).ToList();
            var allPlayerMatchesDict = _matchRepository
                .GetAllMatchesByIds(allScores.Select(score =>  new BsonValue(score.MatchId)))
                .ToDictionary( match => match.MatchId, match => match);

            var serverDict = new Dictionary<string,int>();
            var gameModeDict = new Dictionary<string,int>();
            var dayDict = new Dictionary<int, int>();

            var lastMatchDate = DateTime.MinValue;      
            var totalMatchesWon = 0;
            var scoreBoard = 0.0;

            foreach (var score in allScores)
            {
                var currentMatch = allPlayerMatchesDict[score.MatchId];

                var serverName = currentMatch.Server.Name;
                serverDict.Increment(serverName);

                var gameModeName = currentMatch.GameMode.Name;
                gameModeDict.Increment(gameModeName);

                totalMatchesWon += score.Position == 1 ? 1 : 0;

                var totalPlayers = currentMatch.ScoreBoard.Count;
                scoreBoard += totalPlayers  == 1 
                                    ?  1 
                                    : (double)(totalPlayers - score.Position)/(totalPlayers - 1);

                var dayKey = currentMatch.Date.GetUniqeKey();
                dayDict.Increment(dayKey);

                lastMatchDate = lastMatchDate.Max(currentMatch.Date);
            }


            var totalMatchesPlayed = allPlayerMatchesDict.Count;
            var favoriteServer = serverDict.KeyOfMaxValue(int.MinValue);
            var uniqueServers = serverDict.Count;
            var favoriteGameMode = gameModeDict.KeyOfMaxValue(int.MinValue);
            var averageScoreBoardPercent = scoreBoard*100/allScores.Count;
            var sumAndMax = dayDict.SumAndMax();
            var maxMatchesPerDay = sumAndMax.Item2;
            var averageMatchesPerDay = sumAndMax.Item1/dayDict.Count;
            var ktd = player.KillsToDeathRatio;

            return null;
        }
    }
}