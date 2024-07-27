using System;

namespace IKhom.ServiceLocatorSystem.Runtime.exceptions
{
    internal class ServiceLocatorException : Exception
    {
        internal ServiceLocatorException()
        {
        }

        internal ServiceLocatorException(string message) : base(message)
        {
        }

        internal ServiceLocatorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}