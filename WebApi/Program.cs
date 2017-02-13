using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.SelfHost;

namespace WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var selfHostConfiguraiton = new HttpSelfHostConfiguration("http://localhost:8080");

            selfHostConfiguraiton.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
            new { id = RouteParameter.Optional }
            );
          
            using (var server = new HttpSelfHostServer(selfHostConfiguraiton))
            {
                server.OpenAsync().Wait();
                Console.ReadLine();
            }
        }
    }  
}
