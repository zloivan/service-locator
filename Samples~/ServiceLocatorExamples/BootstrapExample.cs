using IKhom.ServiceLocatorSystem.Runtime;
using ServiceLocatorSystem.Samples.ServiceLocatorExamples.MockServices;
using UnityEngine;

namespace ServiceLocatorSystem.Samples.ServiceLocatorExamples
{
    public class AppLevelBootstrapExample : MonoBehaviour
    {
        private void Awake()
        {
            var loggingServiceExample = new LoggingServiceExample();
            ServiceLocator.Global.Register<ILoggingService>(loggingServiceExample);
            ServiceLocator.Global.Register<IGameManagerService>(new GameManagerServiceExample(loggingServiceExample));
            ServiceLocator.Global.Register<IPlayerService>(new PlayerServiceExample(loggingServiceExample));
            ServiceLocator.ForSceneOf(this).Register<IFirstSceneServiceOnly>(new FirstSceneServiceOnlyExample());
        }
    }
}