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
        public IHttpActionResult AdvertiseServer(string data, string endPoint)
        {
            var model = JsonConvert.DeserializeObject<ServerModel>(data);
            if (model.IsNotValid())
                return BadRequest();
            model.EndPoint = endPoint;
            _serverService.UpsertServer(model);
            return Ok();
        }

        [Route("{endPoint}/matches/{timestamp}")]
        [HttpPut]
        public IHttpActionResult PutMatches(string data, string endPoint, string timestamp)
        {
            var model = JsonConvert.DeserializeObject<MatchModel>(data);
            if (model.IsNotValid())
                return BadRequest();
            var server = _serverRepository.GetByEndPointWithInclude(endPoint);
            if (server == null)
            {
                return BadRequest();
            }
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

        [Route("{endPoint}/stats")]
        [HttpGet]
        public IHttpActionResult GetStatsByEndPoint(string endPoint)
        {

            var server = _serverRepository.GetByEndPointWithInclude(endPoint);
            if (server == null)
                return NotFound();
            var stats = _serverService.GetServerStats(server);
           
            return Ok(
              stats
                );
        }

    }
}


