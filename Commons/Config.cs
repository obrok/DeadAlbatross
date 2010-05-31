using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DeadAlbatross.Commons
{
    public class Config
    {
        private static string _formatString = "http://{0}:{1}/DeadAlbatross/{2}/";
        private static string _port = ConfigurationSettings.AppSettings["Port"];

        private static string Format(object address, object port, object suffix)
        {
            return String.Format(_formatString, address, port, suffix);
        }

        public static Uri BaseAddress(object suffix)
        {
            return new Uri(Format("localhost", _port, suffix));
        }

        public static Uri ServerBaseAddress(object address)
        {
            return new Uri(
                Format(address, _port, "Server") + 
                ConfigurationSettings.AppSettings["ServerSuffix"]);
        }

        public static Uri ClientBaseAddress(object address)
        {
            return new Uri(
                Format(address, _port, "Client") + 
                ConfigurationSettings.AppSettings["ClientSuffix"]);
        }
    }
}
