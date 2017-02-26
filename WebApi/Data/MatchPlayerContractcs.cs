using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WebApi.Data
{
    class MatchPlayerContract
    {
        public int MatchPlayerContractId { get; set; }
        public int MatchId { get; set; }
        [BsonIndex(true)]
        public int PlayerId { get; set; }
        public int ScoreId { get; set; }
    }
}
