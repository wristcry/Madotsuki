using System;
using System.Collections.Generic;
using System.Text;

namespace madotsuki.config {
    public struct map_pair {
        public string key;
        public object value;
        public map_pair(string key, object value) {
            this.key = key;
            this.value = value;
        }
    }
}
