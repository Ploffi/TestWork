using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public class ScoreRepository
    {
        public void Insert(IEnumerable<Score> scores)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {  
                db.GetCollection<Score>(Config.ScoreCol)
                    .Insert(scores);
            }
        }

        public IEnumerable<Score> GetScoreBoardByIds(IEnumerable<BsonValue> scoreBoard)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Score>(Config.ScoreCol)
                    .Include(x => x.Player)
                    .Find(Query.In("_id", scoreBoard));
            }
        }

        public IEnumerable<Score> GetScoreBoardByPlayerId(int playerId)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Score>(Config.ScoreCol)
                    .Include(x => x.Player)
                    .Find(Query.EQ("Player.$id", playerId));
            }
        }
    }
}
