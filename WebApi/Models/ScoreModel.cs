using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ScoreModel
    {
        public int ScoreId { get; set; }
        public string Name { get; set; }
        public int? Frags { get; set; }
        public int? Kills { get; set; }
        public int? Deaths { get; set; }
        public int Position { get; set; }

        public bool IsNotValid()
        {
            return string.IsNullOrEmpty(Name) || 
                   !Frags.HasValue || !Kills.HasValue ||
                   !Deaths.HasValue;
        }
    }
}
