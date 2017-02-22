using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class GameMode
    {
        public int GameModeId { get; set; }
        public string Name { get; set; }

        public GameMode()
        {
            
        }
        public GameMode(string name)
        {
            Name = name;
        }
    }
}
