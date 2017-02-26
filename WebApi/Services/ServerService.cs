using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Services
{
    public class ServerService
    {
        private ServerRepository _serverRepository;
        private GameModeRepository _gameModeRepository;
        private MatchRepository _matchRepository;
        public ServerService()
        {
            _serverRepository = new ServerRepository();
            _gameModeRepository = new GameModeRepository();
            _matchRepository = new MatchRepository();
        }

        public void UpsertServer(ServerModel model)
        {
            var creator = 
            var gameModes = _gameModeRepository.GetOrInsertByName(model.GameModes);
            var server = new Server()
            {
                GameModes = gameModes,
                EndPoint = model.EndPoint,
                Name = model.Name
            };
            _serverRepository.Upsert(server);

        }


        public object GetServerStats(Server server)
        {
            var matches = _matchRepository.GetAllMatchesOnServerId(server.ServerId).ToList();

            var matchPerDayRate = new Dictionary<int,int>();
            Func<DateTime, int> uniqeKey = date => date.DayOfYear + date.Year*1000;
            var gameModeRate = new Dictionary<string, int>();
            var mapsRate = new Dictionary<string, int>();
            var sumPopulation = 0;
            var maxPopulation = 0;

            foreach( var match in matches)
            {
                var key = uniqeKey(match.Date);
                Increase(matchPerDayRate,key);

                var gameModeName = match.GameMode.Name;
                Increase(gameModeRate, gameModeName);

                var mapName = match.Map.Name;
                Increase(mapsRate, mapName);

                sumPopulation += match.ScoreBoard.Count;
                maxPopulation = Math.Max(maxPopulation,match.ScoreBoard.Count);
              
            }

            var maxMatch = matchPerDayRate.Values.Max();
            var average = matchPerDayRate.Values.Sum()/matchPerDayRate.Count;

            var averagePopulation = sumPopulation/ matches.Count;

            var top5Maps = mapsRate.OrderByDescending(pair => pair.Value)
                .Take(5)
                .Select(pair => pair.Key);

            var top5GameModes = gameModeRate.OrderByDescending(pair => pair.Value)
                .Take(5)
                .Select(pair => pair.Key);

            return null;
        }

        private void Increase<T>(Dictionary<T, int> dict, T key)
        {
            if (dict.ContainsKey(key))
                dict[key]++;
            else
                dict.Add(key,1);
        }

    }
}
