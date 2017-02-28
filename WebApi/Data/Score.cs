using LiteDB;

namespace WebApi.Data
{
    public class Score
    {
        public int ScoreId { get; set; }

        [BsonRef("players")]
        [BsonIndex(true)]
        public Player Player { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int MatchId { get; set; }
        public int Position { get; set; }

        [BsonIgnore]
        public Match Match { get; set; }
    }
}
