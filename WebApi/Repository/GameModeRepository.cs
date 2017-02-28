using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using WebApi.Data;

namespace WebApi.Repository
{
    public class GameModeRepository: Repository<GameMode>
    {
        public List<GameMode> GetOrInsertByName(IEnumerable<string> names, Func<string, GameMode> creator)
        {
            List<GameMode> modesList;
            using (var db = new LiteDatabase(Config.DbPath))
            {
                var modesCollection = db.GetCollection<GameMode>(Config.GameModesCol);
                using (var trans = db.BeginTrans())
                {
                    modesList = names.Select(name =>
                    {
                        var exist = modesCollection.FindOne(Query.EQ("Name", name));
                        if (exist == null)
                        {
                            exist = creator(name);
                            modesCollection.Insert(exist);
                        }
                        return exist;
                    }).ToList();

                    trans.Commit();
                }
               
            }
            return modesList;
        }

        public GameMode GetByName(string gameModeName)
        {
            using (var db = new LiteDatabase(Config.ReadOnlyMode))
            {
                return db.GetCollection<GameMode>(Config.GameModesCol)
                    .FindOne(Query.EQ("Name", gameModeName));
            }
        }
    }
}
