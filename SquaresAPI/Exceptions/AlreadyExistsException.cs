using System;
using System.Runtime.Serialization;

namespace SquaresAPI.Exceptions
{
    [Serializable]
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException()
        {
        }

        public AlreadyExistsException(string message) : base(message)
        {
        }

        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}