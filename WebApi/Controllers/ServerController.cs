using System;
using System.Linq;
using System.Web.Http;
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
        public ServerController()
        {
            _matchService = new MatchService();
            _serverService = new ServerService();
            _serverRepository = new ServerRepository();
        }

        [Route("{endPoint}/info/")]
        [HttpPut]
        public IHttpActionResult AdvertiseServer(ServerModel model, string endPoint)
        {
            if (model.IsNotValid())
                return BadRequest();
            model.EndPoint = endPoint;
            _serverService.UpsertServer(model);
            return Ok();
        }

        [Route("{endPoint}/matches/{timestamp}")]
        [HttpPut]
        public IHttpActionResult PutMatches(MatchModel model, string endPoint, string timestamp)
        {
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


