using System;

namespace Bullet4Unity {
    public class UnsupportedWorldTypeException : Exception {
        public UnsupportedWorldTypeException() : base() { }

        public UnsupportedWorldTypeException(string message) : base(message) { }

        public UnsupportedWorldTypeException(string message, Exception inner) : base(message, inner) { }
    }
}
