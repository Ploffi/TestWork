using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    class MatchService
    {
        public void FillDataBase(int count)
        {
            var server = new Server()
            {
                EndPoint = "endpoint",
                Name = "name"
            };
            var idx = 0;
            var gameModesCollection = new GameMode[3] {new GameMode(), new GameMode(), new GameMode() };
            var col = Enumerable.Range(1, count).Select(id => new Match()
            {
                FragLimit = 2,
                GameMode = gameModesCollection[idx++ % 3],
                Server = server,
                TimeElapsed = 2.6,
                TimeLimit = 30
            }).ToList();
            var path = ConfigurationManager.AppSettings["dbPath"];
            var stope = new Stopwatch();
            stope.Start();
            using (var db = new LiteDatabase(path))
            {
                var servers = db.GetCollection<Server>("servers");
                var gameModes = db.GetCollection<GameMode>("game_modes");
                var matches = db.GetCollection<Match>("matches");

               ;
                Console.WriteLine(stope.ElapsedMilliseconds);
                foreach (var se in servers.Include(x => x.EndPoint).FindAll())
                {
                    Console.WriteLine(se.EndPoint);

                }
               
            }
            stope.Stop();
        }
    }
}
