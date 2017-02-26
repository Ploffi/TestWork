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
        public List<GameMode> GetOrInsertByName(IEnumerable<string> gameModes,Func<> )
        {
            return base.GetOrInsertByTCreator(gameModes, Config.GameModesCol, Config.JournalOff,
                name => new GameMode() {Name = name});
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
