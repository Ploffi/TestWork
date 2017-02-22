using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.SelfHost;
using WebApi.Services;

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
                FillData();
                Console.WriteLine("ready");
                Console.ReadLine();
            }
        }

        private static void FillData()
        {
            var service = new MatchService();
            service.FillDataBase(10);
        }
    }  
}
