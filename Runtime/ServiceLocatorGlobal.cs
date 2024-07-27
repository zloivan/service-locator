using IKhom.ServiceLocatorSystem.Runtime.abstractions;
using UnityEngine;

namespace IKhom.ServiceLocatorSystem.Runtime
{
    /// <summary>
    /// Configures the service container to be globally accessible.
    /// </summary>
    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public sealed class ServiceLocatorGlobal : ServiceLocatorBootstrapper
    {
        [SerializeField]
        private bool _dontDestroyOnLoad = true;

        /// <summary>
        /// Configures the service container as a global instance.
        /// </summary>
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(_dontDestroyOnLoad);
        }
    }
}