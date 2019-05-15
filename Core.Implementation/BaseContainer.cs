using System;
using System.Collections.Generic;
using System.Linq;

namespace OrlemSoftware.Basics.Core.Implementation
{
    abstract class BaseContainer
    {
        private volatile object _lockObject = new object();
        private readonly Dictionary<Type, Type> _dependenciesDictionary = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _runningSingletones = new Dictionary<Type, object>();

        protected IDictionary<Type, Type> GetDependenciesDictionary()
        {
            return _dependenciesDictionary;
        }

        protected bool IsTypeRegistered(Type type)
        {
            return _dependenciesDictionary.Keys.Contains(type)
                   || _runningSingletones.Keys.Contains(type);
        }

        protected object GetSingletone(Type type)
        {
            return _runningSingletones.ContainsKey(type)
                ? _runningSingletones[type]
                : null;
        }

        protected void Register(Type abstraction, Type implementation)
        {
            lock (_lockObject)
            {
                if (implementation.GetConstructors().Any())
                    _dependenciesDictionary[abstraction] = implementation;
                else
                    throw new InvalidOperationException($"Constructor not found. Type: {implementation.FullName}");
            }
        }

        protected void Register(Type abstraction, object instance)
        {
            lock (_lockObject)
                _runningSingletones[abstraction] = instance;
        }

        public IReadOnlyCollection<Type> GetRegisteredTypes()
        {
            return _dependenciesDictionary.Keys.Union(_runningSingletones.Keys).ToList();
        }

        protected IDictionary<Type, object> GetRunningSingletones()
        {
            return _runningSingletones;
        }
    }
}
