using IKhom.ServiceLocatorSystem.Runtime.extensions;
using UnityEngine;

namespace IKhom.ServiceLocatorSystem.Runtime.abstractions
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class ServiceLocatorBootstrapper : MonoBehaviour
    {
        internal ServiceLocator Container => _container.OrNull() ?? (_container = GetComponent<ServiceLocator>());
        private ServiceLocator _container;
        private bool _hasBeenBootstrapped;

        private void Awake()
        {
            BootstrapOnDemand();
        }

        /// <summary>
        /// Bootstraps the service container on demand.
        /// </summary>
        public void BootstrapOnDemand()
        {
            if (_hasBeenBootstrapped) return;
            _hasBeenBootstrapped = true;
            Bootstrap();
        }

        /// <summary>
        /// Method to configure the service container. Must be overridden in derived classes.
        /// </summary>
        protected abstract void Bootstrap();
    }
}