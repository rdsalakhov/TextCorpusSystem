using System;
using System.Runtime.Serialization;

namespace TextCorpusSystem
{
    // TODO: refactor this
    [Serializable]
    internal class InvalidAnnotationException : Exception
    {
        public InvalidAnnotationException()
        {
        }

        public InvalidAnnotationException(string message) : base(message)
        {
        }

        public InvalidAnnotationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAnnotationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}