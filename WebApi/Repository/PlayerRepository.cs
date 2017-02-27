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
    public class PlayerRepository:Repository<Player>
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
            List<Player> playerList;

            using (var db = new LiteDatabase(config))
            {
                var playersCollection = db.GetCollection<Player>(Config.PlayersCol);
                playerList = scores.Select(score =>
                {
                    var exist = playersCollection.FindOne(Query.EQ("Name", score.Name));
                    if (exist == null)
                    {
                        exist = creator(score);
                        playersCollection.Insert(exist);
                    }
                    else
                    {
                        updater(score, exist);
                        playersCollection.Update(exist);
                    }
                    return exist;
                }).ToList();
            }
            return playerList;
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