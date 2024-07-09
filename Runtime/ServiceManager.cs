using System;
using System.Collections.Generic;
using ServiceLocator.Runtime.helpers;
using Utilities.ServiceLocator.exceptions;

namespace Utilities.ServiceLocator
{
    /// <summary>
    /// Manages the registration and retrieval of services.
    /// </summary>
    public class ServiceManager
    {
        private readonly object _lock = new object();
        private readonly Dictionary<Type, object> _services = new();
        private readonly ILogger _logger = new ServiceLocatorLogger();

        /// <summary>
        /// Gets all registered services.
        /// </summary>
        public IEnumerable<object> RegisteredServices
        {
            get
            {
                lock (_lock)
                {
                    return _services.Values;
                }
            }
        }

        /// <summary>
        /// Retrieves a registered service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service of type T.</returns>
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (_services.TryGetValue(type, out var obj))
                {
                    _logger.Log($"ServiceManager.Get: Retrieved service of type {type.FullName}");
                    return obj as T;
                }
            }

            throw new ServiceLocatorException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        /// <summary>
        /// Registers a service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The ServiceManager instance.</returns>
        public ServiceManager Register<T>(T service)
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (!_services.TryAdd(type, service))
                {
                    _logger.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
                }
                else
                {
                    _logger.Log($"ServiceManager.Register: Registered service of type {type.FullName}");
                }
            }

            return this;
        }

        /// <summary>
        /// Registers a service with a specific type.
        /// </summary>
        /// <param name="type">The type to register the service as.</param>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The ServiceManager instance.</returns>
        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
                throw new ServiceLocatorException("Type of service does not match type of service interface",
                    new ArgumentException(nameof(service)));

            lock (_lock)
            {
                if (!_services.TryAdd(type, service))
                {
                    _logger.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
                }
                else
                {
                    _logger.Log($"ServiceManager.Register: Registered service of type {type.FullName}");
                }
            }

            return this;
        }

        /// <summary>
        /// Tries to retrieve a registered service of type T.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <param name="service">The output service instance if found.</param>
        /// <returns>True if the service was found, otherwise false.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            lock (_lock)
            {
                if (_services.TryGetValue(type, out var obj))
                {
                    service = obj as T;
                    _logger.Log($"ServiceManager.TryGet: Successfully retrieved service of type {type.FullName}");
                    return true;
                }
            }

            service = null;
            _logger.LogWarning($"ServiceManager.TryGet: Service of type {type.FullName} not found");
            return false;
        }
    }
}