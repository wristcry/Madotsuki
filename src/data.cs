using System;
using System.IO;
using madotsuki.config;

namespace madotsuki {
    public class data {
        static readonly string DATA_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data");
        static readonly string CONFIG_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/config.owo");
        static readonly string SERVERS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/servers.owo");
        static readonly string LOGCHANNELS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/logchannels.owo");
        static readonly string ALLOWED_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/allowedusers.owo");

        static readonly map configowo = new map(CONFIG_PATH);
        static readonly map serversowo = new map(SERVERS_PATH);
        static readonly map logchannelsowo = new map(LOGCHANNELS_PATH);
        static readonly vector allowedusersowo = new vector(ALLOWED_PATH);

        public static string token { get; private set; }
        public static string prefix { get; private set; }
        public static string ownerid { get; private set; }
        public static int trash_cooldown_sec { get; private set; }

        public static void init() {
            if (!Directory.Exists(DATA_PATH))
                Directory.CreateDirectory(DATA_PATH);

            configowo.init(new map_pair[4] { 
                new map_pair("token", "put_ur_token_here"),
                new map_pair("prefix", '!'),
                new map_pair("ownerid", "put_ur_discord_id_here"),
                new map_pair("trash_cooldown_sec", 1800)
            });
            serversowo.init();
            logchannelsowo.init();
            allowedusersowo.init();

            token = configowo.get_value<string>("token");
            prefix = configowo.get_value<string>("prefix");
            ownerid = configowo.get_value<string>("ownerid");
            trash_cooldown_sec = configowo.get_value<int>("trash_cooldown_sec");
        }

        public static bool add_server(string name, ulong id) {
            return data.serversowo.add(new map_pair(name, id));
        }

        public static bool remove_server(string name) {
            return data.serversowo.remove(name);
        }

        public static bool add_logchannel(ulong server_id, ulong channel_id) {
            return data.logchannelsowo.add(new map_pair(server_id.ToString(), channel_id));
        }

        public static bool remove_logchannel(ulong server_id) {
            return data.logchannelsowo.remove(server_id.ToString());
        }

        public static ulong get_logchannel(ulong server_id) {
            object get = data.logchannelsowo.get_value<ulong>(server_id.ToString());
            return get != null ? (ulong)get : (ulong)0;
        }

        public static bool contains_logchannel(ulong server_id) {
            return data.logchannelsowo.contains(server_id.ToString());
        }

        public static bool is_allowed(ulong user_id) {
            return data.allowedusersowo.contains(user_id);
        }

        public static bool add_allowed(ulong user_id) {
            return data.allowedusersowo.add(user_id);
        }

        public static bool remove_allowed(ulong user_id) {
            return data.allowedusersowo.remove(user_id);
        }
    }
}
