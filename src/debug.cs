using System;
using System.Text;
using System.IO;

namespace madotsuki {
    public class debug {
        private static readonly string LOGS_PATH = Path.Combine(Directory.GetCurrentDirectory(), "data/logs");

        public static void init() {
            if (!Directory.Exists(LOGS_PATH))
                Directory.CreateDirectory(LOGS_PATH);
        }

        public static void log(string msg, thread? current_thread = null) {
            string path = (Path.Combine(Directory.GetCurrentDirectory(), LOGS_PATH + "/" + DateTime.Now.Date.Year.ToString() + "-" + DateTime.Now.Date.Month.ToString() + "-" + DateTime.Now.Date.Day.ToString() + ".log"));

            if (!File.Exists(path)) {
                var file = File.Create(path);
                file.Close();
            }

            if (current_thread == null) current_thread = utils.find_thread(0);
            string print = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + " [" + ((thread)current_thread).name + "/" + ((thread)current_thread).id + "] " + msg;
            Console.WriteLine(print);

            using (StreamWriter file = new StreamWriter(path, true)) {
                file.WriteLine(print);
                file.Close();
            }
            current_thread = null;
        }
    }
}
