using System;
using System.Threading;

namespace madotsuki {
    public struct thread {
        public string name;
        public int id;
        public Thread t;

        public thread(string name, int id, Thread t) {
            this.name = name;
            this.id = id;
            this.t = t;
        }

        public thread(string name, int id, Action method) {
            this.name = name;
            this.id = id;
            this.t = new Thread(new ThreadStart(method));
        }
    }
}
