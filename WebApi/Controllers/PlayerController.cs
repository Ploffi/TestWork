using System.Web;
using System.Web.Http;

using WebApi.Services;

namespace WebApi.Controllers
{
    [RoutePrefix("api/players")]
    public class PlayerController: ApiController
    {
        private PlayerService _playerService;
        public PlayerController()
        {
            _playerService = new PlayerService();
        }

        [Route("{name}/stats")]
        public IHttpActionResult GetPlayerStats(string name)
        {
           
            name = HttpUtility.UrlDecode(name);
            if (string.IsNullOrEmpty(name))
                return BadRequest();
            var stats = _playerService.GetPlayerStats(name.ToLowerInvariant());
            if (stats == null)
                return BadRequest();
            return Ok(stats);
        }
    }
}
