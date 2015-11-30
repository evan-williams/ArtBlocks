using System;
using System.Configuration;
using System.Linq;

namespace LTI.API.Client
{
    public class Config
    {
        public static Config Current { get; private set; }

        public string APIAuthority { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessTokenKey { get; set; }
        public string AccessTokenSecret { get; set; }
        public string RequestTokenUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string AuthoriseUrl { get; set; }

        static Config()
        {
            Current = ParseConfigFile();
        }

        private Config()
        {    
        }

        private static Config ParseConfigFile()
        {
            var config = new Config();
            var properties = config.GetType().GetProperties().ToList();
            properties.ForEach(x =>
            {
                if (x.PropertyType == typeof (string))
                {
                    x.SetValue(config, ParseString(x.Name), null);
                }
                else if (x.PropertyType == typeof(int))
                {
                    x.SetValue(config, ParseInteger(x.Name), null);
                }
                else if (x.PropertyType == typeof(long))
                {
                    x.SetValue(config, ParseLong(x.Name), null);
                }
                else if (x.PropertyType == typeof(DateTime))
                {
                    x.SetValue(config, ParseDateTime(x.Name), null);
                }
            });

            return config;
        }

        private static string ParseString(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName];
        }

        private static int ParseInteger(string keyName)
        {
            return int.Parse(ConfigurationManager.AppSettings[keyName]);
        }

        private static long ParseLong(string keyName)
        {
            return long.Parse(ConfigurationManager.AppSettings[keyName]);
        }

        private static DateTime ParseDateTime(string keyName)
        {
            return DateTime.Parse(ConfigurationManager.AppSettings[keyName]);
        }
    }
}
