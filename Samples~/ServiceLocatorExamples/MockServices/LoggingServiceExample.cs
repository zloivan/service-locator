using UnityEngine;

namespace ServiceLocatorSystem.Samples.ServiceLocatorExamples.MockServices
{
    public interface ILoggingService
    {
        void Log(string message);
    }
    
    public class LoggingServiceExample : ILoggingService
    {
        public void Log(string message)
        {
            Debug.Log($"LoggingService: {message}");
        }
    }
}