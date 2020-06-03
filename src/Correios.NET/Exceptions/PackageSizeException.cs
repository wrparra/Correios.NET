using System;
using System.Runtime.Serialization;

namespace Correios.NET.Exceptions
{
    [Serializable]
    public class PackageSizeException : Exception
    {
        public PackageSizeException()
        {
        }

        public PackageSizeException(string message) : base(message)
        {
        }

        public PackageSizeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PackageSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}