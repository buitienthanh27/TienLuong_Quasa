using System.Runtime.Serialization;

namespace API_Sample.Utilities
{
    public class DataException : Exception
    {
        public DataException()
        {
        }

        public DataException(string message) : base(message)
        {
        }

        public DataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
