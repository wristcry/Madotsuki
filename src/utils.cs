using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using System.Linq;

namespace madotsuki {
    public class utils {
        public static long startTime;
        public static double uptime;

        public static void init() {
            startTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            Program._threads[1] = new thread("Uptime Thread", 1, count_uptime);
            Program._threads[2] = new thread("GC Thread", 2, collect_trash);
        }

        public static string get_uptime() {
            TimeSpan t = TimeSpan.FromSeconds(uptime);
            return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", 
                t.Days,
                t.Hours,
                t.Minutes,
                t.Seconds);
        }

        public static thread? find_thread(string name) {
            for (int i = 0; i < Program._threads.Length; i++)
                if (Program._threads[i].name == name) return Program._threads[i];

            return null;
        }

        public static thread? find_thread(int id) {
            for (int i = 0; i < Program._threads.Length; i++)
                if (Program._threads[i].id == id) return Program._threads[i];

            return null;
        }

        public static bool is_windows() { 
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool is_linux() {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        public static bool is_osx() {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        public static bool is_supported() {
            return !is_osx() && (is_windows() || is_linux());
        }

        public static string get_OS_name() {
            return RuntimeInformation.OSDescription;
        }

        public static ulong get_mem_amount() {
            if (!is_linux()) {
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject item in moc)
                    return Convert.ToUInt64(item.Properties["TotalPhysicalMemory"].Value) / 1024;
            }
            else {
                string line = File.ReadAllLines("/proc/meminfo")[0];
                line = line.Replace("MemTotal:", string.Empty).Replace("kB", string.Empty);
                line = new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
                return Convert.ToUInt64(line);
            }
            
            return 0;
        }

        // i mean, cmon microsoft, add #define, im fuckin bored of making things like that
        public static double count_percents(double a, double b) {
            return Math.Round(a / b * 100.0, 1);
        }

        static void count_uptime() {
            for (; ; ) {
                Thread.Sleep(1000);
                uptime += 1.0;
            }
        }

        static void collect_trash() {
            for (; ; ) {
                Thread.Sleep(data.trash_cooldown_sec * 1000);
                debug.log("Collecting trash...", find_thread("GC Thread"));
                GC.Collect();
                debug.log("Trash collected", find_thread("GC Thread"));
            }
        }
    }
}
