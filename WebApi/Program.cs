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
using System.Web.Http.Results;
using System.Web.Http.SelfHost;
using LiteDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApi.Data;
using WebApi.Services;

namespace WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
           // TestHuynu();
            var selfHostConfiguraiton = new HttpSelfHostConfiguration("http://localhost:8080");

            selfHostConfiguraiton.MapHttpAttributeRoutes();
            using (var server = new HttpSelfHostServer(selfHostConfiguraiton))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("ready");
                Console.ReadLine();

                Console.WriteLine("end huyna");
            }
        }

        private static Random random;

        private static  string _adress = "http://localhost:8080/api";
        private static HttpWebRequest CreateHttp(string methdod, string uri, string parameters)
        {
            var webr = (HttpWebRequest) WebRequest.Create($"{_adress}/{uri}");
            webr.Timeout = 1000;
            webr.Method = methdod;
            if (parameters != null)
            {
                webr.ContentLength = parameters.Length;
                var newstr = webr.GetRequestStream();
                var bytes = Encoding.Unicode.GetBytes(parameters);
                newstr.Write(bytes,0,bytes.Length);
            }
            return webr;
        }

        private static IEnumerable<HttpWebRequest> GenerateScript(Random rand,int count)
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
                scoreboard =
                    new PlayerData[] {new PlayerData() {deaths = 12, kills = 14, name = rand.Next().ToString()},}
            };
            var parameters = JsonConvert.SerializeObject(match);
            return Enumerable.Range(1, count).Select(num => CreateHttp(method, uri, parameters));
        }

        private static PlayerData[] create(Random rand)
        {
            return Enumerable.Range(1, rand.Next(1, 5)).Select(num => new PlayerData()
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
}
