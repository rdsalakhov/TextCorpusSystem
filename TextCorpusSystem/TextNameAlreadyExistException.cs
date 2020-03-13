using System;
using System.Runtime.Serialization;

namespace TextCorpusSystem
{
    [Serializable]
    public class TextNameAlreadyExistException : Exception
    {
        public TextNameAlreadyExistException()
        {
        }

        public TextNameAlreadyExistException(string message) : base(message)
        {
        }

        public TextNameAlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TextNameAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}