using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi
{
    public static class Config
    {
        static NameValueCollection settings;
        static Config()
        {
            settings = ConfigurationManager.AppSettings;
        }

        public static string DbPath => settings["DbPath"];
        public static string ServersCol => settings["ServersCol"];
        public static string PlayersCol => settings["PlayersCol"];
        public static string ScoreCol => settings["ScoreCol"];
        public static string MatchCol => settings["MatchCol"];
        public static string GameModesCol => settings["GameModesCol"];
        public static string JournalOff => "filename="+DbPath +settings["JournalOff"];
        public static string MapsCol => settings["MapsCol"];
        public static string MPContract => settings["MPContract"];
    }
}
