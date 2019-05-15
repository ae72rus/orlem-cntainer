using System;
using System.Collections.Generic;
using OrlemSoftware.Basics.Core.Implementation.Exceptions;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Basics.Core.Implementation
{
    class RegionManager : IRegionManager
    {
        private readonly Dictionary<Type, RegistratorResolver> _registratorResolversByRegions = new Dictionary<Type, RegistratorResolver>();

        private readonly ILoggingService _loggingService;
        public RegionManager(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public void Register<TInterface, TImplementation, TRegion>() where TImplementation : TInterface where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion), true);
            regRes.Register<TInterface, TImplementation>();
        }

        public void Register<TInterface, TRegion>(TInterface instance) where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion), true);
            regRes.Register(instance);
        }

        public void RegisterFactory<TFactory, TInterface, TImplementation, TRegion>()
            where TFactory : IFactory where TImplementation : TInterface where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion), true);
            regRes.RegisterFactory<TFactory, TInterface, TImplementation>();
        }

        public TInterface Resolve<TInterface, TRegion>() where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.Resolve<TInterface>();
        }

        public TInterface Resolve<TInterface, TRegion>(params object[] args) where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.Resolve<TInterface>(args);
        }

        public TInterface Resolve<TInterface, TRegion>(params ResolveParameter[] parameters) where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.Resolve<TInterface>(parameters);
        }

        public object Resolve<TRegion>(Type type) where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.Resolve(type);
        }

        public object Resolve<TRegion>(Type type, params object[] args) where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.Resolve(type, args);
        }

        public object Resolve<TRegion>(Type type, params ResolveParameter[] parameters) where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.Resolve(type, parameters);
        }

        public IReadOnlyCollection<Type> GetRegisteredTypes<TRegion>() where TRegion : IRegion
        {
            var regRes = getRegistratorResolverByRegion(typeof(TRegion));
            return regRes.GetRegisteredTypes();
        }

        private RegistratorResolver getRegistratorResolverByRegion(Type regionType, bool canAddRegion = false)
        {
            if (_registratorResolversByRegions.ContainsKey(regionType))
                return _registratorResolversByRegions[regionType];

            if (canAddRegion)
                _registratorResolversByRegions[regionType] = new RegistratorResolver(_loggingService);
            else
                throw new RegionNotFoundException($"Region: {regionType.FullName}");

            return _registratorResolversByRegions[regionType];
        }

        public RegionManager Clone(ILoggingService loggingService)
        {
            var retv = new RegionManager(_loggingService);
            foreach (var registratorResolversByRegion in _registratorResolversByRegions)
            {
                retv._registratorResolversByRegions.Add(registratorResolversByRegion.Key, registratorResolversByRegion.Value);
            }

            return retv;
        }
    }
}