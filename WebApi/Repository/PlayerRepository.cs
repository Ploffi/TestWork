using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public class PlayerRepository:Repository<Player>
    {

        public Player GetByName(string playerName)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Player>(Config.PlayersCol)
                    .FindOne(Query.EQ("Name", playerName));
            }
        }

        public List<Player> GetOrInsertByScoreModel(IEnumerable<ScoreModel> scores,
            Func<ScoreModel, Player> creator , Action<ScoreModel,Player> updater)
        {
            List<Player> playerList;

            using (var db = new LiteDatabase(Config.DbPath))
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
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Player>(Config.PlayersCol)
                    .Find(Query.In("_id", ids));
            }
        }


        public IEnumerable<Player> GetBest(int count,int minTotalDeaths = 0,int minTotalMatchesPlayed = 10)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Player>(Config.PlayersCol)
                    .Find(Query.And(
                        Query.GT("TotalDeaths",minTotalDeaths),
                        Query.GT("TotalMatchesPlayed",minTotalMatchesPlayed)
                        ),0,count);
            }
        }

    }
}