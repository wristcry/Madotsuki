using System;

namespace madotsuki {
    public class OwnerAssertationFailedException : Exception {
        public OwnerAssertationFailedException() { }
    }

    public class UserAssertationFailedException : Exception {
        public UserAssertationFailedException() { }
    }
}
