using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Web.Http.Tracing;
using WebApi.JsonConvertors;
using WebApi.Utils;

namespace WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var uriData = args.Skip(1).First().Split('+');           
            var prefix = uriData.First();
            var port = uriData.Last();

            var selfHostConfiguraiton = Configure(prefix+"localhost"+port);
            selfHostConfiguraiton.MapHttpAttributeRoutes();
            
            using (var server = new HttpSelfHostServer(selfHostConfiguraiton))
            {
                server.OpenAsync().Wait();
                Console.ReadKey(true);
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
