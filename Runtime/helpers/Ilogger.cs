using UnityEngine;

namespace IKhom.ServiceLocatorSystem.Runtime.helpers
{
    internal interface ILogger
    {
        public void Log(object message, Object context = null);
        public void LogWarning(object message, Object context = null);
        public void LogError(object message, Object context = null);
    }

    internal class ServiceLocatorLogger : ILogger
    {
        public void Log(object message, Object context)
        {
#if DEBUG_SERVICE_LOCATOR
            if (Debug.isDebugBuild || Application.isEditor)
            {
                Debug.Log($"<b>SERVICE_LOCATOR:</b> {message}", context);
            }
#endif
        }

        public void LogWarning(object message, Object context = null)
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                Debug.LogWarning($"<b>SERVICE_LOCATOR:</b> {message}", context);
            }
        }

        public void LogError(object message, Object context = null)
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                Debug.LogError($"<b>SERVICE_LOCATOR:</b> {message}", context);
            }
        }
    }
}