using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Correios.NET.Exceptions
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception inner) : base(message, inner) { }
       
        protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
