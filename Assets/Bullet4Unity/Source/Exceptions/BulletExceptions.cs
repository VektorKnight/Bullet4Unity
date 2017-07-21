using System;

namespace Bullet4Unity {
    /// <summary>
    /// Thrown when a world doesn't support the object that attempted to register with it
    /// </summary>
    public class UnsupportedWorldTypeException : Exception {
        public UnsupportedWorldTypeException() { }
        public UnsupportedWorldTypeException(string message) : base(message) { }
        public UnsupportedWorldTypeException(string message, Exception inner) : base(message, inner) { }
    }
    
    /// <summary>
    /// Thrown when an object attempts to register with an invalid world entry
    /// </summary>
    public class InvalidWorldNameException : Exception {
        public InvalidWorldNameException () { }
        public InvalidWorldNameException(string message) : base(message) { }
        public InvalidWorldNameException(string message, Exception inner) : base(message, inner) { }
    }
}
