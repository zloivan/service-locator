using UnityEngine;
using Utilities.ServiceLocator.abstractions;

namespace Utilities.ServiceLocator
{
    /// <summary>
    /// Configures the service container specific to a scene.
    /// </summary>
    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public sealed class ServiceLocatorScene : ServiceLocatorBootstrapper
    {
        /// <summary>
        /// Configures the service container for the current scene.
        /// </summary>
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}