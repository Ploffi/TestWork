using LiteDB;

namespace WebApi.Data
{
    public class Map
    {
        public int MapId { get; set; }
        [BsonIndex(true)]
        public string Name { get; set; }
    }
}
