using System;
using System.Linq;
using System.Web.Http;
using LiteDB;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController: ApiController
    {
        private MatchRepository _matchRepository;
        private PlayerRepository _playerRepository;
        private ServerService _serverService;

        public ReportsController()
        {
            _matchRepository = new MatchRepository();
            _playerRepository = new PlayerRepository();
            _serverService = new ServerService();            
        }

        [Route("recent-matches/{count?}")]
        [HttpGet]
        public IHttpActionResult GetRecentMatches(int count = 5)
        {

            count = Math.Min(count, 50);
            if (count <= 0)
                return Ok(new object[0]);

            var matches = _matchRepository.GetRecentMatches(count)
                .ToList();
            var scores = matches.SelectMany(m => m.ScoreBoard).ToList();
            var playersDict = _playerRepository
                .GetByIds(scores
                    .Select(sc =>  new BsonValue(sc.Player.PlayerId)))
                .ToDictionary(player => player.PlayerId, player => player);

            foreach (var score in scores)
            {
                score.Player = playersDict[score.Player.PlayerId];
            }

            return Ok(matches);
        }

        [Route("best-players/{count?}")]
        public IHttpActionResult GetBestPlayers(int count = 5)
        {
            count = Math.Min(count, 50);
            if (count <= 0)
                return Ok(new object[0]);

            var players = _playerRepository.GetBest(count,0,10);
            return Ok(
                players.OrderByDescending(player => player.KillsToDeathRatio)
            );
        }

        [Route("popular-servers/{count?}")]
        public IHttpActionResult GetPopularServers(int count = 5)
        {
            count = Math.Min(count, 50);
            if (count <= 0)
                return Ok(new object[0]);

            var serversInfo = _serverService.GetPopular(count);
            return Ok(
               serversInfo
            );
        }

    }

}
