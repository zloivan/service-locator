using System;
using System.Collections.Generic;
using System.Linq;
using ServiceLocator.Runtime.helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.ServiceLocator.exceptions;
using Utilities.ServiceLocator.extensions;
using ILogger = ServiceLocator.Runtime.helpers.ILogger;

namespace Utilities.ServiceLocator
{
    /// <summary>
    /// Service Locator for managing service registration and retrieval at global or scene level.
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private const string K_GLOBAL_SERVICE_LOCATOR_NAME = "ServiceLocator [Global]";
        private const string K_SCENE_SERVICE_LOCATOR_NAME = "ServiceLocator [Scene]";
        private static ServiceLocator _global;
        private static Dictionary<Scene, ServiceLocator> _sceneContainers;
        private static List<GameObject> _tmpSceneGameObjects;

        private static ILogger _logger = new ServiceLocatorLogger();
        private static readonly object _lock = new();

        private readonly ServiceManager _services = new();

        /// <summary>
        /// Gets the global ServiceLocator instance. Creates new if none exists.
        /// </summary>
        public static ServiceLocator Global
        {
            get
            {
                lock (_lock)
                {
                    if (_global != null)
                        return _global;

                    if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
                    {
                        found.BootstrapOnDemand();
                        return _global;
                    }

                    var container = new GameObject(K_GLOBAL_SERVICE_LOCATOR_NAME, typeof(ServiceLocator));
                    container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();
                    _logger.Log("Global ServiceLocator created.", container);
                    return _global;
                }
            }
        }

        /// <summary>
        /// Gets the closest ServiceLocator instance to the provided MonoBehaviour in hierarchy, the ServiceLocator for its scene, or the global ServiceLocator.
        /// </summary>
        /// <param name="mb">The MonoBehaviour to find the ServiceLocator for.</param>
        /// <returns>The closest ServiceLocator instance.</returns>
        public static ServiceLocator For(MonoBehaviour mb)
        {
            lock (_lock)
            {
                var locator = mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
                _logger.Log($"ServiceLocator.For: Retrieved ServiceLocator for {mb.name}", locator);
                return locator;
            }
        }

        /// <summary>
        /// Returns the ServiceLocator configured for the scene of a MonoBehaviour. Falls back to the global instance.
        /// </summary>
        /// <param name="mb">The MonoBehaviour to find the scene's ServiceLocator for.</param>
        /// <returns>The ServiceLocator instance for the scene.</returns>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            var scene = mb.gameObject.scene;
            lock (_lock)
            {
                if (_sceneContainers.TryGetValue(scene, out var container) && container != mb)
                {
                    _logger.Log($"ServiceLocator.ForSceneOf: Retrieved scene ServiceLocator for {mb.name}", container);
                    return container;
                }

                _tmpSceneGameObjects.Clear();
                scene.GetRootGameObjects(_tmpSceneGameObjects);

                foreach (var go in _tmpSceneGameObjects.Where(go => go.GetComponent<ServiceLocatorScene>() != null))
                {
                    if (!go.TryGetComponent(out ServiceLocatorScene bootstrapper) || bootstrapper.Container == mb)
                        continue;

                    bootstrapper.BootstrapOnDemand();
                    
                    _logger.Log($"ServiceLocator.ForSceneOf: Bootstrapped scene ServiceLocator for {mb.name}",
                        bootstrapper);
                    return bootstrapper.Container;
                }

                _logger.LogWarning(
                    $"ServiceLocator.ForSceneOf: No scene ServiceLocator found for {mb.name}, using Global.", mb);
                return Global;
            }
        }

        /// <summary>
        /// Gets a service of a specific type. If no service of the required type is found, an error is thrown.
        /// </summary>
        /// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
        /// <param name="service">Service of type T to get.</param>
        /// <returns>The ServiceLocator instance after attempting to retrieve the service.</returns>
        public ServiceLocator Get<T>(out T service) where T : class
        {
            lock (_lock)
            {
                if (TryGetService(out service))
                {
                    _logger.Log($"ServiceLocator.Get: Retrieved service of type {typeof(T).FullName}", this);
                    return this;
                }

                if (!TryGetNextInHierarchy(out var container))
                    throw new ServiceLocatorException(
                        $"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");

                container.Get(out service);
                _logger.Log($"ServiceLocator.Get: Retrieved service of type {typeof(T).FullName} from hierarchy",
                    container);
                return this;
            }
        }

        /// <summary>
        /// Allows retrieval of a service of a specific type. An error is thrown if the required service does not exist.
        /// </summary>
        /// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
        /// <returns>Instance of the service of type T.</returns>
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (TryGetService(type, out T service))
                {
                    _logger.Log($"ServiceLocator.Get: Retrieved service of type {typeof(T).FullName}", this);
                    return service;
                }

                if (!TryGetNextInHierarchy(out var container))
                    throw new ServiceLocatorException($"Could not resolve type '{typeof(T).FullName}'.");

                _logger.Log($"ServiceLocator.Get: Retrieved service of type {typeof(T).FullName} from hierarchy",
                    container);

                return container.Get<T>();
            }
        }

        /// <summary>
        /// Registers a service to the ServiceLocator using the service's type.
        /// </summary>
        /// <typeparam name="T">Class type of the service to be registered.</typeparam>
        /// <param name="service">The service to register.</param>
        /// <returns>The ServiceLocator instance after registering the service.</returns>
        public ServiceLocator Register<T>(T service)
        {
            lock (_lock)
            {
                _services.Register(service);
                _logger.Log($"ServiceLocator.Register: Registered service of type {typeof(T).FullName}", this);
                return this;
            }
        }

        /// <summary>
        /// Registers a service to the ServiceLocator using a specific type.
        /// </summary>
        /// <param name="type">The type to use for registration.</param>
        /// <param name="service">The service to register.</param>
        /// <returns>The ServiceLocator instance after registering the service.</returns>
        public ServiceLocator Register(Type type, object service)
        {
            lock (_lock)
            {
                _services.Register(type, service);
                _logger.Log($"ServiceLocator.Register: Registered service of type {type.FullName}", this);
                return this;
            }
        }

        /// <summary>
        /// Tries to get a service of a specific type. Returns whether or not the process is successful.
        /// </summary>
        /// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
        /// <param name="service">Service of type T to get.</param>
        /// <returns>True if the service retrieval was successful, false otherwise.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            lock (_lock)
            {
                var success = _services.TryGet(out service) ||
                              TryGetNextInHierarchy(out var container) && container.TryGet(out service);
                if (success)
                {
                    _logger.Log($"ServiceLocator.TryGet: Successfully retrieved service of type {typeof(T).FullName}",
                        this);
                }
                else
                {
                    _logger.LogWarning($"ServiceLocator.TryGet: Service of type {typeof(T).FullName} not found", this);
                }

                return success;
            }
        }

        /// <summary>
        /// Configures this ServiceLocator as the global instance.
        /// </summary>
        /// <param name="dontDestroyOnLoad">If true, the ServiceLocator will persist across scene loads.</param>
        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            lock (_lock)
            {
                if (_global == this)
                {
                    _logger.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
                }
                else if (_global != null)
                {
                    _logger.LogError(
                        "ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global",
                        this);
                }
                else
                {
                    _global = this;
                    if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
                    _logger.Log("ServiceLocator.ConfigureAsGlobal: Configured as global", this);
                }
            }
        }

        /// <summary>
        /// Configures this ServiceLocator for the current scene.
        /// </summary>
        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;
            lock (_lock)
            {
                if (_sceneContainers.ContainsKey(scene))
                {
                    _logger.LogError(
                        "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene",
                        this);
                    return;
                }

                _sceneContainers.Add(scene, this);
                _logger.Log($"ServiceLocator.ConfigureForScene: Configured for scene {scene.name}", this);
            }
        }

        private void OnDestroy()
        {
            lock (_lock)
            {
                if (this == _global)
                {
                    _global = null;
                    _logger.Log("ServiceLocator.OnDestroy: Global ServiceLocator destroyed", this);
                }
                else if (_sceneContainers.ContainsValue(this))
                {
                    _sceneContainers.Remove(gameObject.scene);
                    _logger.Log($"ServiceLocator.OnDestroy: Scene ServiceLocator for {gameObject.scene.name} destroyed",
                        this);
                }
            }
        }

        // https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            lock (_lock)
            {
                _global = null;
                _sceneContainers = new Dictionary<Scene, ServiceLocator>();
                _tmpSceneGameObjects = new List<GameObject>();
                _logger = new ServiceLocatorLogger();
                _logger.Log("ServiceLocator.ResetStatics: Static fields reset", null);
            }
        }

        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == _global)
            {
                container = null;
                return false;
            }

            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
            return container != null;
        }

        private bool TryGetService<T>(out T service) where T : class
        {
            return _services.TryGet(out service);
        }

        private bool TryGetService<T>(Type type, out T service) where T : class
        {
            return _services.TryGet(out service);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobal()
        {
            var go = new GameObject(K_GLOBAL_SERVICE_LOCATOR_NAME, typeof(ServiceLocatorGlobal));
            _logger.Log("ServiceLocator.AddGlobal: Added Global ServiceLocator from menu", go);
        }

        [UnityEditor.MenuItem("GameObject/ServiceLocator/Add Scene")]
        private static void AddScene()
        {
            var go = new GameObject(K_SCENE_SERVICE_LOCATOR_NAME, typeof(ServiceLocatorScene));
            _logger.Log("ServiceLocator.AddScene: Added Scene ServiceLocator from menu", go);
        }
#endif
    }
}