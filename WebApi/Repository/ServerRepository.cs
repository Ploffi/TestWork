using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Data;

namespace WebApi.Repository
{
    public class ServerRepository
    {
        public void Upsert(Server server)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                var collection = db.GetCollection<Server>(Config.ServersCol);
                var exist = collection.FindOne(Query.EQ("EndPoint", server.EndPoint));
                if (exist == null)
                {
                    collection.Insert(server);
                }
                else
                {
                    server.ServerId = exist.ServerId;
                    collection.Update(server);
                }
            }
        }

        public Server GetByEndPointWithInclude(string endPoint)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                return db.GetCollection<Server>(Config.ServersCol)
                    .Include(x => x.GameModes)
                    .FindOne(Query.EQ("EndPoint", endPoint));
            }
        }

        public Server GetById(int serverId)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                return db.GetCollection<Server>(Config.ServersCol)
                    .FindById(serverId);
            }
        }

        public IEnumerable<Server> GetAllWithInclude()
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                return db.GetCollection<Server>(Config.ServersCol)
                    .Include(s => s.GameModes)
                    .FindAll();
            }
        }

      
    }
}
