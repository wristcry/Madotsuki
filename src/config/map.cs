using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace madotsuki.config {
    public class map {
        string path;

        public map(string path) {
            this.path = path;
        }

        public void init() {
            if (!File.Exists(path)) File.WriteAllText(path, string.Empty);
            else return;
        }

        public void init(map_pair[] pairs) {
            if (!File.Exists(path)) {
                string output = string.Empty;
                for (int i = 0; i < pairs.Length; i++)
                    output += (pairs[i].key + '=' + pairs[i].value.ToString() + '\n');
                File.WriteAllText(path, output);
                output = null;
            }
            else return;
        }

        public bool contains(string key) {
            bool result;
            string file = File.ReadAllText(path);
            result = file.Contains(key + '=');
            file = null;
            return result;
        }

        public bool add(map_pair pair) {
            if (contains(pair.key)) return false;

            string file = File.ReadAllText(path);
            File.Delete(path);
            if (file.Length > 0 && file[file.Length - 1] != '\n') file += '\n';
            file += (pair.key + '=' + pair.value.ToString() + '\n');
            File.WriteAllText(path, file);
            file = null;
            return true;
        }

        public bool remove(string key) {
            if (!contains(key)) return false;

            List<string> l = new List<string>();
            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                l.Add(lines[i]);
            file = null;
            lines = null;
            for (int i = 0; i < l.Count; i++)
                if (l[i].Contains(key + '=')) l.RemoveAt(i);
            string output = string.Empty;
            for (int i = 0; i < l.Count; i++) {
                if (String.IsNullOrEmpty(l[i]) || String.IsNullOrWhiteSpace(l[i])) continue;
                output += (l[i] + '\n');
            }
            l.Clear();
            l = null;
            File.WriteAllText(path, output);
            output = null;
            return true;
        }

        public object get_object(string key) {
            object result = null;
            if (!contains(key)) return result;

            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            file = null;
            for (int i = 0; i < lines.Length; i++) {
                string[] data = lines[i].Split('=');
                if (data.Length == 2 && data[0] == key) {
                    result = data[1].Replace("\n", string.Empty);
                    break;
                }
            }
            return result;
        }

        public string get_string(string key) {
            string result = null;
            if (!contains(key)) return result;

            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            file = null;
            for (int i = 0; i < lines.Length; i++) {
                string[] data = lines[i].Split('=');
                if (data.Length == 2 && data[0] == key) {
                    result = data[1].Replace("\n", string.Empty);
                    break;
                }
            }
            return result;
        }

        public int get_int(string key) {
            int result = new int();
            if (!contains(key)) return result;

            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            file = null;
            for (int i = 0; i < lines.Length; i++) {
                string[] data = lines[i].Split('=');
                if (data.Length == 2 && data[0] == key) {
                    result = Convert.ToInt32(data[1].Replace("\n", string.Empty));
                    break;
                }
            }
            return result;
        }

        public uint get_uint(string key) {
            uint result = new uint();
            if (!contains(key)) return result;

            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            file = null;
            for (int i = 0; i < lines.Length; i++) {
                string[] data = lines[i].Split('=');
                if (data.Length == 2 && data[0] == key) {
                    result = Convert.ToUInt32(data[1].Replace("\n", string.Empty));
                    break;
                }
            }
            return result;
        }

        public long get_long(string key) {
            long result = new long();
            if (!contains(key)) return result;

            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            file = null;
            for (int i = 0; i < lines.Length; i++) {
                string[] data = lines[i].Split('=');
                if (data.Length == 2 && data[0] == key) {
                    result = Convert.ToInt64(data[1].Replace("\n", string.Empty));
                    break;
                }
            }
            return result;
        }

        public ulong get_ulong(string key) {
            ulong result = new ulong();
            if (!contains(key)) return result;

            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            file = null;
            for (int i = 0; i < lines.Length; i++) {
                string[] data = lines[i].Split('=');
                if (data.Length == 2 && data[0] == key) {
                    result = Convert.ToUInt64(data[1].Replace("\n", string.Empty));
                    break;
                }
            }
            return result;
        }

        public T get_value<T>(string key) {
            if (typeof(T) == typeof(string)) return (T)(object)get_string(key);
            else if (typeof(T) == typeof(int)) return (T)(object)get_int(key);
            else if (typeof(T) == typeof(uint)) return (T)(object)get_uint(key);
            else if (typeof(T) == typeof(long)) return (T)(object)get_long(key);
            else if (typeof(T) == typeof(ulong)) return (T)(object)get_ulong(key);
            else return (T)get_object(key);
        }

        public bool set_value(string key, object value) {
            if (!contains(key)) return false;

            List<string> l = new List<string>();
            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                l.Add(lines[i]);
            file = null;
            lines = null;
            for (int i = 0; i < l.Count; i++) {
                if (l[i].Contains(key + '=')) {
                    string[] data = l[i].Split('=');
                    if (data.Length == 2 && data[0] == key) {
                        data[1] = value.ToString();
                        l[i] = data[0] + '=' + data[1];
                    }
                }
            }
            string output = string.Empty;
            for (int i = 0; i < l.Count; i++)
                output += (l[i] + '\n');
            l.Clear();
            l = null;
            File.WriteAllText(path, output);
            output = null;
            return true;
        }
    }
}
