using System;

namespace Utilities.ServiceLocator.exceptions
{
    public class ServiceLocatorException : Exception
    {
        public ServiceLocatorException()
        {
        }

        public ServiceLocatorException(string message) : base(message)
        {
        }

        public ServiceLocatorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}