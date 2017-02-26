using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using LiteDB;
using Newtonsoft.Json;
using WebApi.JsonConvertors;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/reports")]
    class ReportsController: ApiController
    {
        private MatchRepository _matchRepository;
        private ScoreRepository _scoreRepository;
        private PlayerRepository _playerRepository;

        public ReportsController()
        {
            _scoreRepository= new ScoreRepository();
            _matchRepository = new MatchRepository();
        }

        [Route("recent-matches/{count}")]
        [HttpGet]
        public IHttpActionResult GetRecentMatches(int count = 5)
        {
            count = Math.Min(count, 50);
            if (count <= 0)
                return Ok(JsonConvert.SerializeObject(new object[0]));

            var matches = _matchRepository.GetRecentMatches(DateTime.MaxValue,count).ToList();
            var scores = matches.SelectMany(m => m.ScoreBoard).ToList();
            var players = _playerRepository.GetByIds(scores.Select(sc =>  new BsonValue(sc.Player.PlayerId))).ToList();

            for (var scIdx = 0;scIdx<scores.Count;scIdx++)
            {
                scores[scIdx].Player = players[scIdx];
            }

            return Ok(JsonConvert.SerializeObject(matches,Formatting.None,new MatchConvertor(),new ScoreConverter()));
        }
    }
}
