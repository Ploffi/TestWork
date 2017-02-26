using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using LiteDB;
using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Services
{
    internal class PlayerRepository:Repository<Player>
    {
        public PlayerRepository()
        {
        }

        public Player GetByName(string playerName)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                return db.GetCollection<Player>(Config.PlayersCol)
                    .FindOne(Query.EQ("Name", playerName));
            }
        }

        public List<Player> GetOrInsertByScoreModel(IEnumerable<ScoreModel> scores,
            Func<ScoreModel, Player> creator , Action<ScoreModel,Player> updater,bool withoutJournal = false)
        {
            var config = withoutJournal ? Config.JournalOff : Config.DbPath;
            Func<ScoreModel,Query> queryCreator = sc => Query.EQ("Name",sc.Name);

            return GetOrInsertByTCreator<ScoreModel>(scores,creator,updater,queryCreator,
                Config.PlayersCol,config);
        }

        public IEnumerable<Player> GetByIds(IEnumerable<BsonValue> ids)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                return db.GetCollection<Player>(Config.PlayersCol)
                    .Find(Query.In("_id", ids));
            }
        }
    }
}