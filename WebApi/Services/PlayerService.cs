using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    internal class PlayerService
    {
        private PlayerRepository _playerRepository;

        public PlayerService()
        {
            _playerRepository = new PlayerRepository();
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
    }
}