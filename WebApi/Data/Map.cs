using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
