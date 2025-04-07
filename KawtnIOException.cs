using System;
using System.Collections.Generic;
using System.Text;

namespace kawtn.IO
{
    public class KawtnIOException : Exception
    {
        public KawtnIOException() { }
        public KawtnIOException(string message) : base(message) { }
        public KawtnIOException(Exception innerException) : base(string.Empty, innerException) { }
        public KawtnIOException(string message, Exception innerException) : base(message, innerException) { }
    }
}
