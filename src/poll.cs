using System;
using System.Collections.Generic;
using System.Text;

namespace madotsuki {
    public struct poll_option {
        public string name;
        public int voted;

        public poll_option(string name, int voted) {
            this.name = name;
            this.voted = voted;
        }
    }

    public struct poll {
        public ulong channelid;
        public long start_time;
        public long estimated_time;
        public string name;
        public poll_option[] options;
        public List<ulong> voters;

        public poll(ulong channelid, int start_time, int estimated_time, string name, poll_option[] options, List<ulong> voters) {
            this.channelid = channelid;
            this.start_time = start_time;
            this.estimated_time = estimated_time;
            this.name = name;
            this.options = options;
            this.voters = voters;
        }
    }
}
