using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Data;

namespace WebApi.Repository
{
    public class GameModeRepository: Repository<GameMode>
    {
        public List<GameMode> GetOrInsertByName(IEnumerable<string> names, Func<string, GameMode> creator,
            bool withoutJournal)
        {
            var config = withoutJournal ? Config.JournalOff : Config.DbPath;
            List<GameMode> modesList;
            using (var db = new LiteDatabase(config))
            {
                var modesCollection = db.GetCollection<GameMode>(Config.GameModesCol);
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
            }
            return modesList;
        }

        public GameMode GetByName(string gameModeName)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<GameMode>(Config.GameModesCol)
                    .FindOne(Query.EQ("Name", gameModeName));
            }
        }
    }
}
