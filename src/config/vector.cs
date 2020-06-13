using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace madotsuki.config {
    public class vector {
        string path;

        public vector(string path) {
            this.path = path;
        }

        public void init() {
            if (!File.Exists(path)) File.WriteAllText(path, string.Empty);
            else return;
        }

        public void init(object[] elements) {
            if (!File.Exists(path)) {
                string output = string.Empty;
                for (int i = 0; i < elements.Length; i++)
                    output += (elements.ToString() + '\n');
                File.WriteAllText(path, output);
                output = null;
            }
            else return;
        }

        public bool contains(object element) {
            bool result;
            string file = File.ReadAllText(path);
            result = file.Contains(element.ToString());
            file = null;
            return result;
        }

        public bool add(object element) {
            if (contains(element)) return false;

            string file = File.ReadAllText(path);
            File.Delete(path);
            if (file.Length > 0 && file[file.Length - 1] != '\n') file += '\n';
            file += (element.ToString() + '\n');
            File.WriteAllText(path, file);
            file = null;
            return true;
        }

        public bool remove(object element) {
            if (!contains(element)) return false;

            List<string> l = new List<string>();
            string file = File.ReadAllText(path);
            string[] lines = file.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                l.Add(lines[i]);
            file = null;
            lines = null;
            for (int i = 0; i < l.Count; i++)
                if (l[i].Contains(element.ToString())) l.RemoveAt(i);
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
