using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Models.Server
{
    class ServerInfo
    {
        int ServerInfoId { get; set; }
        public string Name { get; set; }
        public string EndPoint { get; set; }

    }
}
