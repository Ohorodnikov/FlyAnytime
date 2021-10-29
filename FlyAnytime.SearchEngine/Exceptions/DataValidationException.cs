using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.SearchEngine.Exceptions
{
    [Serializable]
    public class DataValidationException : Exception
    {
        public DataValidationException() { }
        public DataValidationException(string message) : base(message) { }
        public DataValidationException(string message, Exception inner) : base(message, inner) { }
        protected DataValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
