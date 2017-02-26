using LiteDB;
using WebApi.Data;

namespace WebApi.Services
{
    public class MapRepository
    {
        public Map GetByName(string name)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                return db.GetCollection<Map>(Config.MapsCol)
                    .FindOne(Query.EQ("Name", name));
            }
        }

        public void Insert(Map map)
        {
            using (var db = new LiteDatabase(Config.JournalOff))
            {
                db.GetCollection<Map>(Config.MapsCol)
                    .Insert(map);
            }
        }
    }
}