using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Data;
using WebApi.JsonConvertors;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;


namespace WebApi.Controllers
{
    [RoutePrefix("api/servers")]
    public class ServerController : ApiController
    {
        private MatchService _matchService;
        private ServerService _serverService;
        private ServerRepository _serverRepository;
        public MatchRepository _matchRepository;
        public ServerController()
        {
            _matchService = new MatchService();
            _serverService = new ServerService();
            _matchRepository = new MatchRepository();
            _serverRepository = new ServerRepository();
        }

        [Route("{endPoint}/info/")]
        [HttpPut]
        public IHttpActionResult AdvertiseServer(JObject data, string endPoint)
        {
            var model = data.ToObject<ServerModel>();
            model.EndPoint = endPoint;
            _serverService.UpsertServer(model);
            return Ok();
        }

        [Route("{endPoint}/matches/{timestamp}")]
        [HttpPut]
        public IHttpActionResult PutMatches(JObject data, string endPoint, string timestamp)
        {

            var server = _serverRepository.GetByEndPointWithInclude(endPoint);
            if (server == null)
            {
                return BadRequest();
            }
            var model = data.ToObject<MatchModel>();
            model.Date = DateTime.Parse(timestamp);
            model.Server = server;


            if (!_matchService.TryInsert(model))
                return BadRequest();

            return Ok();
        }

        [Route("{endPoint}/info")]
        [HttpGet]
        public IHttpActionResult GetInfoByEndPoint(string endPoint)
        {

            var server = _serverRepository.GetByEndPointWithInclude(endPoint);
            if (server == null)
                return NotFound();
           
            return Ok(server);
        }

        [Route("info")]
        [HttpGet]
        public IHttpActionResult GetServersInfo()
        {
            var servers = _serverRepository.GetAllWithInclude().ToList();
            return Ok(servers);
        }

        [Route("{endPoint}/matches/{timestamp}")]
        [HttpGet]
        public IHttpActionResult GetMatchInfo(string endPoint,string timestamp)
        {
            DateTime time;
            if (!DateTime.TryParse(timestamp,out time))
                return BadRequest();
            
            var match = _matchService.GetMatchByTimeStampOnServer(endPoint, time);
            
            if (match == null)
                return NotFound();

            return Ok( match );
        }

        [Route("test/{count}")]
        [HttpGet]
        public IHttpActionResult TestGetServer(int count)
        {
            
            var test = new test();
            Console.Clear();
            Console.WriteLine("start test");
            test.TestHuynu(count);
            Console.WriteLine("end test");
            return Ok();
        }

        [Route("{endPoint}/stats")]
        [HttpGet]
        public IHttpActionResult GetStatsByEndPoint(string endPoint)
        {

            var server = _serverRepository.GetByEndPointWithInclude(endPoint);
            if (server == null)
                return NotFound();
            var stats = _serverService.GetServerStats(server);
            var top5 = new [] {new GameMode() {Name = "second"}, new GameMode() {Name = "second"}};
            return Ok(
              stats
                );
        }

    }


    #region test
    class test
    {
        Random random;
        public  void TestHuynu(int count)
        {
            random = new Random();        
            List<long> spent = new List<long>(count);
            var stop = new Stopwatch();
            foreach (var l in GenerateScript(random, count))
            {
                try
                {
                   
                    stop.Start();
                    l.GetResponse();
                    stop.Stop();
                    spent.Add(stop.ElapsedMilliseconds);
                    stop.Reset();
                }
                catch (Exception e)
                {
                    stop.Reset();
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("end");
            Console.WriteLine("average " + spent.Sum() / spent.Count);
            Console.WriteLine("max " + spent.Max());
            Console.WriteLine("min " + spent.Min());
            foreach (var sp in spent.Take(10))
            {
                Console.Write($" {sp}");
            }
            spent.Sort();
            Console.WriteLine("median " + spent[spent.Count / 2]);
           



        }
        private string _adress = "http://localhost:8080/api";
        private HttpWebRequest CreateHttp(string methdod, string uri, string parameters)
        {
            var webr = (HttpWebRequest)WebRequest.Create($"{_adress}/{uri}");
            webr.Timeout = 4000;
            webr.Method = methdod;
            webr.ContentType = "application/json";
            if (parameters != null)
            {
                webr.ContentLength = parameters.Length;
                var newstr = webr.GetRequestStream();
                var bytes = Encoding.UTF8.GetBytes(parameters);
                newstr.Write(bytes, 0, bytes.Length);
            }
            return webr;
        }

        private IEnumerable<HttpWebRequest> GenerateScript(Random rand, int count)
        {
            var uri = "servers/endPointTest/matches/" + DateTime.Now;
            var method = "PUT";
            var match = new MatchesData()
            {
                map = "mapa",
                gameMode = "DM",
                fragLimit = 12,
                timeLimit = 10,
                timeElapsed = 12.5,
                scoreboard = create(random)                
            };
            var parameters = JsonConvert.SerializeObject(match);
            return Enumerable.Range(1, count).Select(num => CreateHttp(method, uri, parameters));
        }

        private PlayerData[] create(Random rand)
        {
            return Enumerable.Range(1, rand.Next(20, 60)).Select(num => new PlayerData()
            {
                deaths = 1,
                frags = 2,
                kills = 3,
                name = rand.Next().ToString()
            }).ToArray();
        }

        private class MatchesData
        {
            public string map;
            public string gameMode;
            public int fragLimit;
            public int timeLimit;
            public double timeElapsed;
            public PlayerData[] scoreboard;

        }
        private class PlayerData
        {
            public string name;
            public int frags;
            public int kills;
            public int deaths;
        }
    }
#endregion
}


