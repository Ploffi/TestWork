using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class Score
    {
        public int ScoreId { get; set; }

        public string Name { get; set; }
        public int Frags { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
