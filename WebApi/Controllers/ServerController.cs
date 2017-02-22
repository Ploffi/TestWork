using System.Web.Http;
using System.Web.Http.Results;
using WebApi.Data;


namespace WebApi.Controllers
{
    [RoutePrefix("servers")]
    public class ServerController : ApiController
    {

        public ServerController()
        {
        }
        [HttpGet]
        public JsonResult<Server> GetServer()
        {
            var s = new Server();
            return Json(s);
        }

        [Route("{endPoint}/info/")]
        [HttpPut]
        public void AdvertiseServer(ServerModel model,string endPoint)
        {
           
        }

    }

}
