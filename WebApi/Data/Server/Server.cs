using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models.Server
{
    public class Server
    {
        public int ServerId { get; set; }
        public string Name { get; set; }
        public string EndPoint { get; set; }

        public virtual ICollection<GameMode> GameModes { get; set; } 
    }
}
