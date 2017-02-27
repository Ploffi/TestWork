using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using LiteDB;
using Newtonsoft.Json;
using WebApi.JsonConvertors;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ReportsConfig]
    [RoutePrefix("api/reports")]
    public class ReportsController: ApiController
    {
        private MatchRepository _matchRepository;
        private ScoreRepository _scoreRepository;
        private PlayerRepository _playerRepository;

        public ReportsController()
        {
            _scoreRepository= new ScoreRepository();
            _matchRepository = new MatchRepository();
            _playerRepository = new PlayerRepository();
            
            
        }

        [Route("recent-matches/{count?}")]
        [HttpGet]
        public IHttpActionResult GetRecentMatches(int count = 5)
        {

            count = Math.Min(count, 50);
            if (count <= 0)
                return Ok(JsonConvert.SerializeObject(new object[0]));

            var matches = _matchRepository.GetRecentMatches(DateTime.MaxValue,count)
                .OrderByDescending(match => match.Date)
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

        [Route("reports/best-players/{count?}")]
        public IHttpActionResult GetBestPlayers(int count = 5)
        {
            return Ok();
        }

    }

    public class ReportsConfig : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings,
                               HttpControllerDescriptor controllerDescriptor)
        {
            var converters = controllerSettings.Formatters.JsonFormatter.SerializerSettings.Converters;
            converters.Add(new RecentMatchesConvertor());
            converters.Add(new MatchConvertor());
            converters.Add(new ScoreConverter());

        }
    }
}
