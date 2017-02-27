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
            var selfHostConfiguraiton = new HttpSelfHostConfiguration("http://localhost:8080");

            selfHostConfiguraiton.MapHttpAttributeRoutes();
            using (var server = new HttpSelfHostServer(selfHostConfiguraiton))
            {
                server.OpenAsync().Wait();
                Console.WriteLine("ready");
                Console.ReadLine();
            }
        }

        private static void TestHuynu()
        {
            var d = new Dictionary<string, int>();
            for (var i = 0; i < 100*100*100; i++)
            {
                d.Add(i.ToString(),i);
            }
            var st = Stopwatch.StartNew();
            
            Console.WriteLine(d.SumAndMax());
            st.Stop();
            Console.WriteLine(st.ElapsedMilliseconds);
            st.Reset();
            st.Start();

            Console.WriteLine();
            st.Stop();
            Console.WriteLine(st.ElapsedMilliseconds);
            st.Reset();

        }
    }  
}
