using System.Collections.Generic;
using LiteDB;
using WebApi.Data;

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
            using (var db = new LiteDatabase(Config.ReadOnlyMode))
            {
                return db.GetCollection<Score>(Config.ScoreCol)
                    .Include(x => x.Player)
                    .Find(Query.In("_id", scoreBoard));
            }
        }

        public IEnumerable<Score> GetScoreBoardByPlayerId(int playerId)
        {
            using (var db = new LiteDatabase(Config.ReadOnlyMode))
            {
                return db.GetCollection<Score>(Config.ScoreCol)
                    .Include(x => x.Player)
                    .Find(Query.EQ("Player.$id", playerId));
            }
        }
    }
}
