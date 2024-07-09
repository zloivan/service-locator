using UnityEngine;

namespace Utilities.ServiceLocator.Examples
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