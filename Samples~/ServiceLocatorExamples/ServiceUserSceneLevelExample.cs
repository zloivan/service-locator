using IKhom.ServiceLocatorSystem.Runtime;
using ServiceLocatorSystem.Samples.ServiceLocatorExamples.MockServices;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ServiceLocatorSystem.Samples.ServiceLocatorExamples
{
    public class ServiceSceneLevelUser : MonoBehaviour
    {
        private void Start()
        {
            //Getting first scene service properly
            var firstSceneServiceOnly = ServiceLocator.For(this).Get<IFirstSceneServiceOnly>();
            firstSceneServiceOnly.DoSomeWork();

            Invoke(nameof(LoadSecondSceneWithDelay), 2f);
        }

        public void LoadSecondSceneWithDelay()
        {
            SceneManager.LoadScene("Second");
        }
    }
}