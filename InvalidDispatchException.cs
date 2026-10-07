using System;

namespace Disaster_Response_Coordination.Exceptions
{
    public class InvalidDispatchException : Exception
    {
        public InvalidDispatchException() : base("The dispatch operation is invalid.") { }
        public InvalidDispatchException(string message) : base(message) { }
        public InvalidDispatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
