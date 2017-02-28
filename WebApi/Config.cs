using System.Collections.Specialized;
using System.Configuration;

namespace WebApi
{
    public static class Config
    {
        static readonly NameValueCollection settings;
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
        public static string JournalOff => DbPath +settings["JournalOff"];
        public static string MapsCol => settings["MapsCol"];
        public static string ReadOnlyMode => JournalOff+settings["ReadOnlyMode"];
        public static string UtilsDb => settings["UtilsDb"];
        public static string UtilsCol => settings["UtilsCol"];
        public static string UtilsDbReadOnly => "filename=" + settings["UtilsDb"] + settings["ReadOnlyMode"];
    }
}
