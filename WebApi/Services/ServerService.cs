using System;
using System.Collections.Generic;
using System.Linq;
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
            Func<string,GameMode> creator = str => new GameMode() { Name = str};
            var gameModes = _gameModeRepository.GetOrInsertByName(model.GameModes,creator);

            var server = new Server()
            {
                GameModes = gameModes,
                EndPoint = model.EndPoint,
                Name = model.Name
            };

            _serverRepository.Upsert(server);
        }


        public ServerStats GetServerStats(Server server)
        {
            var matches = _matchRepository.GetAllMatchesOnServerId(server.ServerId).ToList();

            var matchPerDayRate = new Dictionary<DateTime,int>();

            var minDate = DateTime.MaxValue;
            var maxDate = DateTime.MinValue;

            var gameModeRate = new Dictionary<string, int>();
            var mapsRate = new Dictionary<string, int>();
            var sumPopulation = 0;
            var maxPopulation = 0;

            foreach( var match in matches)
            {
                var date = match.Date.Date;
                matchPerDayRate.Increment(date);

                minDate = date.Min(minDate);
                maxDate = date.Max(maxDate);

                var gameModeName = match.GameMode.Name;
                gameModeRate.Increment(gameModeName);

                var mapName = match.Map.Name;
                mapsRate.Increment(mapName);

                sumPopulation += match.ScoreBoard.Count;
                maxPopulation = Math.Max(maxPopulation,match.ScoreBoard.Count);
              
            }


            return matches.Count == 0  
                ? new ServerStats()
                : new ServerStats()
            {
                totalMatchesPlayed = matches.Count,
                maximumMatchesPerDay = matchPerDayRate.Values.Max(),
                averageMatchesPerDay = (double) matches.Count/(maxDate.Subtract(minDate).Days + 1),
                maximumPopulation = maxPopulation,
                averagePopulation = (double) sumPopulation/matches.Count,
                top5GameModes = gameModeRate.OrderByDescending(pair => pair.Value)
                    .Take(5)
                    .Select(pair => pair.Key).ToArray(),
                top5Maps = mapsRate.OrderByDescending(pair => pair.Value)
                    .Take(5)
                    .Select(pair => pair.Key).ToArray()

            };

        }


        public IEnumerable<ServerInfo> GetPopular(int count)
        {
            var matches = _matchRepository.GetAll();
            var serversDict = _serverRepository.GetAll()
                .ToDictionary(server => server.ServerId, server => new ServerInfo()
            {
                AverageMatchesPerDay = 0,
                Name = server.Name,
                EndPoint = server.EndPoint
            });
            var serverMinDateDict = new Dictionary<int,DateTime>(serversDict.Count); 
            var serversTotalMatchesDict = new Dictionary<int,int>(serversDict.Count); 

            var maxDate = DateTime.MinValue;
            foreach (var match in matches)
            {
                var serverId = match.ServerId;
                if (serverMinDateDict.ContainsKey(match.ServerId))
                {
                    serverMinDateDict[serverId] = match.Date.Date.Max(serverMinDateDict[serverId]);
                }
                else
                {
                    serverMinDateDict[serverId] = match.Date.Date;
                }
                maxDate = maxDate.Max(match.Date.Date);
                serversTotalMatchesDict.Increment(serverId);
            }

            foreach (var serverMinDate in serverMinDateDict)
            {
                var serverId = serverMinDate.Key;

                serversDict[serverId].AverageMatchesPerDay = 
                    (double) serversTotalMatchesDict[serverId] / (maxDate.Subtract(serverMinDate.Value).Days + 1);
            }

            return serversDict.Values.Take(count);
        }
    }

  
}
