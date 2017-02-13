using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using WebApi.Models.Server;

namespace WebApi.Controllers
{
    public class ServerController : ApiController
    {
        public JsonResult<Server> GetServer()
        {
            var s = new Server();
            return Json(s);
        }
    }
}
