using LiteDB;

namespace WebApi.Data
{
    public class GameMode
    {
        public int GameModeId { get; set; }
        [BsonIndex(true)]
        public string Name { get; set; }
    }
}
