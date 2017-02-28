using System;
using System.Linq;
using LiteDB;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Repository
{
    public class UtilRepository
    {
        public void UpdateMaxDate(DateTime date)
        {
            using (var db = new LiteDatabase(Config.UtilsDb))
            {
                var coll = db.GetCollection<DateInfo>(Config.UtilsCol);

                var dateBd = coll.FindOne(Query.All());
                if (dateBd == null)
                {
                    var dateInfo = new DateInfo() {Date = date};
                    coll.Insert(dateInfo);
                    return;
                }
                dateBd.Date = date.Max(dateBd.Date);
                coll.Update(dateBd);
            }
        }

        public DateTime GetMaxDate()
        {
            using (var db = new LiteDatabase(Config.UtilsDbReadOnly))
            {
                return db.GetCollection<DateInfo>(Config.UtilsCol)
                    .FindAll().First().Date;

            }
        }
    }
}
