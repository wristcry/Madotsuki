using System;
using System.IO;

namespace madotsuki {
    public class config {
        private static readonly string CONFIG_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/config.owo");

        public static string token;
        public static string prefix;
        public static string ownerid;
        public static int trash_cooldown_sec;

        public static void init() {
            if (!File.Exists(CONFIG_PATH))
                File.WriteAllText(CONFIG_PATH, "token=1337\nprefix=!\nownerid=1337\ntrash_cooldown_sec=600");

            string[] cfgdata = File.ReadAllText(CONFIG_PATH).Split('\n');
            for (int i = 0; i < cfgdata.Length; i++) {
                if (cfgdata[i].Contains("token=")) token = cfgdata[i].Replace("token=", string.Empty);
                else if (cfgdata[i].Contains("prefix=")) prefix = cfgdata[i].Replace("prefix=", string.Empty);
                else if (cfgdata[i].Contains("ownerid=")) ownerid = cfgdata[i].Replace("ownerid=", string.Empty);
                else if (cfgdata[i].Contains("trash_cooldown_sec=")) trash_cooldown_sec = Convert.ToInt32(cfgdata[i].Replace("trash_cooldown_sec=", string.Empty));
            }
        }
    }
}
