using UnityEngine;

namespace ServiceLocatorSystem.Samples.ServiceLocatorExamples.MockServices
{
    public interface IFirstSceneServiceOnly
    {
        void DoSomeWork();
    }

    public class FirstSceneServiceOnlyExample : IFirstSceneServiceOnly
    {
        public void DoSomeWork()
        {
            Debug.Log("Service is registered for first scene only");
        }
    }
}