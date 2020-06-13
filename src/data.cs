using System;
using System.IO;

namespace madotsuki {
    public class data {
        private static readonly string DATA_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data");
        private static readonly string SERVERS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/servers.owo");
        private static readonly string LOGCHANNELS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/logchannels.owo");
        private static readonly string ALLOWED_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/allowedusers.owo");

        public static string[] allowed_users;

        public static void init() {
            if (!Directory.Exists(DATA_PATH))
                Directory.CreateDirectory(DATA_PATH);

            if (!File.Exists(SERVERS_PATH))
                File.WriteAllText(SERVERS_PATH, string.Empty);

            if (!File.Exists(LOGCHANNELS_PATH))
                File.WriteAllText(LOGCHANNELS_PATH, string.Empty);

            if (!File.Exists(ALLOWED_PATH))
                File.WriteAllText(ALLOWED_PATH, string.Empty);

            if (new FileInfo(ALLOWED_PATH).Length > 0) allowed_users = File.ReadAllText(ALLOWED_PATH).Split('\n');
            else allowed_users = new string[1];
        }

        public static bool server_add(string name, string id) {
            if (File.ReadAllText(SERVERS_PATH).Contains(id)) return false;
            using (StreamWriter file = new StreamWriter(SERVERS_PATH, true)) {
                file.WriteLine(name + ":" + id);
                file.Close();
            }
            return true;
        }

        // Todo
        public static bool server_remove(string nameorid) {
            string[] content = File.ReadAllLines(SERVERS_PATH);
            File.Delete(SERVERS_PATH);
            File.WriteAllText(SERVERS_PATH, string.Empty);
            for (int i = 0; i < content.Length; i++) {
                if (!content[i].Contains(nameorid)) {
                    using (StreamWriter file = new StreamWriter(SERVERS_PATH, true)) {
                        file.WriteLine(content[i]);
                        file.Close();
                    }
                }
            }
            return false;
        }

        public static bool logchannel_add(string serverid, string channelid) {
            if (File.ReadAllText(LOGCHANNELS_PATH).Contains(serverid)) return false;
            using (StreamWriter file = new StreamWriter(LOGCHANNELS_PATH, true)) {
                file.WriteLine(serverid + ":" + channelid);
                file.Close();
            }
            return true;
        }

        // Todo
        public static bool logchannel_remove(string serverorchannelid) {
            string[] content = File.ReadAllLines(LOGCHANNELS_PATH);
            File.Delete(LOGCHANNELS_PATH);
            File.WriteAllText(LOGCHANNELS_PATH, string.Empty);
            for (int i = 0; i < content.Length; i++) {
                if (!content[i].Contains(serverorchannelid)) {
                    using (StreamWriter file = new StreamWriter(LOGCHANNELS_PATH, true)) {
                        file.WriteLine(content[i]);
                        file.Close();
                    }
                }
            }
            return true;
        }

        public static bool logchannel_get(string serverid, out ulong channelid) {
            string temp = null;
            string[] content = File.ReadAllLines(LOGCHANNELS_PATH);
            for (int i = 0; i < content.Length; i++)
                if (content[i].Contains(serverid)) temp = content[i].Replace(":", string.Empty).Replace(serverid, string.Empty).Replace("\n", string.Empty);
            ulong result;
            if (temp != null) {
                channelid = Convert.ToUInt64(temp);
                return true;
            }
            else {
                channelid = (ulong)1337;
                return false;
            }
        }

        public static ulong logchannel_get(string serverid) {
            string temp = null;
            string[] content = File.ReadAllLines(LOGCHANNELS_PATH);
            for (int i = 0; i < content.Length; i++)
                if (content[i].Contains(serverid)) temp = content[i].Replace(":", string.Empty).Replace(serverid, string.Empty).Replace("\n", string.Empty);
            if (temp != null) return Convert.ToUInt64(temp);
            else return 0;
        }
    }
}
