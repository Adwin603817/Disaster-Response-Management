using System;

namespace Disaster_Response_Coordination.Exceptions
{
    public class ResourceUnavailableException : Exception
    {
        public ResourceUnavailableException() : base("The requested resource is currently unavailable.") { }
        public ResourceUnavailableException(string message) : base(message) { }
        public ResourceUnavailableException(string message, Exception innerException) : base(message, innerException) { }
    }
}