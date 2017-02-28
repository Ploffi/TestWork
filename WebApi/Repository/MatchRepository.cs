﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public  class MatchRepository
    {
        public void Insert(Match match)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                db.GetCollection<Match>(Config.MatchCol)
                    .Insert(match);
            }
        }

        public void Update(Match match)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                db.GetCollection<Match>(Config.MatchCol)
                    .Update(match);
            }
        }

        public Match GetMatchByTimeStampOnServer(string endPoint, DateTime timeStamp)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                var serverId = db.GetCollection<Server>(Config.ServersCol)
                    .FindOne(Query.EQ("EndPoint", endPoint)).ServerId;

                var match = db.GetCollection<Match>(Config.MatchCol)
                    .Include(m => m.GameMode)
                    .Include(m => m.Map)
                    .FindOne(Query.EQ("Date", timeStamp));           

                return match != null && serverId == match.Server.ServerId
                    ? match
                    : null;
            }
        }

        public IEnumerable<Match> GetAllMatchesOnServerId(int serverId)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Match>(Config.MatchCol)
                    .Include(m => m.GameMode)
                    .Include(m => m.ScoreBoard)
                    .Include(m => m.Map)
                    .Find(Query.EQ("ServerId",serverId));
            }
        }


        public IEnumerable<Match> GetRecentMatches(int count)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Match>(Config.MatchCol)
                    .Include(m => m.ScoreBoard)
                    .Include(m => m.Map)
                    .Include(m => m.GameMode)
                    .Include(m => m.Server)
                    .Find(Query.All("Date",Query.Descending),0,count)
                    ;
            }
        }

        public IEnumerable<Match> GetAllMatchesByIds(IEnumerable<BsonValue> ids)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Match>(Config.MatchCol)
                    .Include(m => m.Map)
                    .Include(m => m.GameMode)
                    .Include(m => m.Server)
                    .Find(Query.In("_id", ids));
            }
        }

        public IEnumerable<Match> GetAll()
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Match>(Config.MatchCol)
                    .FindAll();
            }
        }
    }
}