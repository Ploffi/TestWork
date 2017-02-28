using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using System.Web.Http.SelfHost;
using System.Web.Http.Tracing;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.JsonConvertors;
using WebApi.Services;
using WebApi.Utils;

namespace WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var selfHostConfiguraiton = Configure("http://localhost:8080");
            selfHostConfiguraiton.MapHttpAttributeRoutes();
            
            using (var server = new HttpSelfHostServer(selfHostConfiguraiton))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("ready");
                Console.ReadLine();
            }
        }

        private static HttpSelfHostConfiguration Configure(string uri)
        {
            var config = new HttpSelfHostConfiguration(uri);
            var convertors = config.Formatters.JsonFormatter.SerializerSettings.Converters;

            convertors.Add(new MatchConvertor());
            convertors.Add(new ScoreConverter());
            convertors.Add(new ServerConvertor());
            convertors.Add(new RecentMatchesConvertor());
            convertors.Add(new PlayerConverter());
            convertors.Add(new ServerListConvertor());

            config.Services.Replace(typeof(ITraceWriter),new NLogger());
            return config;
        }

        
    }  
}
