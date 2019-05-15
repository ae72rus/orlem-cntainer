using System;
using System.Collections.Generic;
using OrlemSoftware.Basics.Core.Attributes;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Basics.Core.Implementation
{
    [Singletone]
    public class OrlemContainer : IContainer
    {
        private readonly ILoggingService _loggingService;
        private readonly string _dynamicAssemblyName;
        private readonly RegionManager _regionManager;
        private readonly GlobalEventsInformer _globalEventsInformer;
        private readonly List<Type> _dependenciesSources = new List<Type>();
        private readonly RegistratorResolver _registratorResolver;

        public OrlemContainer(ILoggingService loggingService) : this(loggingService, null)
        {

        }

        public OrlemContainer(ILoggingService loggingService, string dynamicAssemblyName)
        {
            _loggingService = loggingService;
            _dynamicAssemblyName = dynamicAssemblyName;
            _registratorResolver = new RegistratorResolver(loggingService, dynamicAssemblyName)
            {
                OnShutdownCreated = onShutdownCreated
            };

            _registratorResolver.Register(loggingService);

            _regionManager = new RegionManager(loggingService);
            _globalEventsInformer = new GlobalEventsInformer(loggingService);
        }
        
        #region registratorResolver
        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _registratorResolver.Register<TInterface, TImplementation>();
        }

        public void Register<TInterface>(TInterface instance)
        {
            _registratorResolver.Register<TInterface>(instance);
        }

        public void Register(Type abstraction, Type implementation)
        {
            _registratorResolver.Register(abstraction, implementation);
        }

        public void RegisterFactory<TFactory, TInterface, TImplementation>() where TFactory : IFactory where TImplementation : TInterface
        {
            _registratorResolver.RegisterFactory<TFactory, TInterface, TImplementation>();
        }

        public TInterface Resolve<TInterface>()
        {
            return _registratorResolver.Resolve<TInterface>();
        }

        public TInterface Resolve<TInterface>(params object[] args)
        {
            return _registratorResolver.Resolve<TInterface>(args);
        }

        public TInterface Resolve<TInterface>(params ResolveParameter[] parameters)
        {
            return _registratorResolver.Resolve<TInterface>(parameters);
        }

        public object Resolve(Type type)
        {
            return _registratorResolver.Resolve(type);
        }

        public object Resolve(Type type, params object[] args)
        {
            return _registratorResolver.Resolve(type, args);
        }

        public object Resolve(Type type, params ResolveParameter[] parameters)
        {
            return _registratorResolver.Resolve(type, parameters);
        }
        #endregion

        #region regionManager
        public void Register<TInterface, TImplementation, TRegion>() where TImplementation : TInterface where TRegion : IRegion
        {
            _regionManager.Register<TInterface, TImplementation, TRegion>();
        }

        public void Register<TInterface, TRegion>(TInterface instance) where TRegion : IRegion
        {
            _regionManager.Register<TInterface, TRegion>(instance);
        }

        public void RegisterFactory<TFactory, TInterface, TImplementation, TRegion>() where TFactory : IFactory where TImplementation : TInterface where TRegion : IRegion
        {
            _regionManager.RegisterFactory<TFactory, TInterface, TImplementation, TRegion>();
        }

        public TInterface Resolve<TInterface, TRegion>() where TRegion : IRegion
        {
            return _regionManager.Resolve<TInterface, TRegion>();
        }

        public TInterface Resolve<TInterface, TRegion>(params object[] args) where TRegion : IRegion
        {
            return _regionManager.Resolve<TInterface, TRegion>(args);
        }

        public TInterface Resolve<TInterface, TRegion>(params ResolveParameter[] parameters) where TRegion : IRegion
        {
            return _regionManager.Resolve<TInterface, TRegion>(parameters);
        }

        public object Resolve<TRegion>(Type type) where TRegion : IRegion
        {
            return _regionManager.Resolve<TRegion>(type);
        }

        public object Resolve<TRegion>(Type type, params object[] args) where TRegion : IRegion
        {
            return _regionManager.Resolve<TRegion>(type, args);
        }

        public object Resolve<TRegion>(Type type, params ResolveParameter[] parameters) where TRegion : IRegion
        {
            return _regionManager.Resolve<TRegion>(type, parameters);
        }

        public IReadOnlyCollection<Type> GetRegisteredTypes<TRegion>() where TRegion : IRegion
        {
            return _regionManager.GetRegisteredTypes<TRegion>();
        }
        #endregion

        #region eventInformer
        public event EventHandler Startup
        {
            add => _globalEventsInformer.Startup += value;
            remove => _globalEventsInformer.Startup -= value;
        }

        public event EventHandler Shutdown
        {
            add => _globalEventsInformer.Shutdown += value;
            remove => _globalEventsInformer.Shutdown -= value;
        }
        #endregion

        public IContainer AddDependenciesSource<TDependenciesSource>() where TDependenciesSource : class, IDependenciesSource, new()
        {
            _dependenciesSources.Add(typeof(TDependenciesSource));
            return this;
        }

        public void Start()
        {
            _registratorResolver.Register<IContainer>(this);
            _registratorResolver.Register<IResolver>(this);
            _registratorResolver.Register<IRegistrator>(this);
            _registratorResolver.Register<IRegionManager>(this);
            _registratorResolver.Register<IRegionRegistrator>(this);
            _registratorResolver.Register<IRegionResolver>(this);
            _registratorResolver.Register<IGlobalEventsInformer>(this);
            foreach (var dependenciesSource in _dependenciesSources)
            {
                var source = _registratorResolver.Resolve(dependenciesSource) as IDependenciesSource;
                source?.SetDependencies(this);
            }

            _globalEventsInformer.RaiseStartup();
        }

        public void Stop()
        {
            _globalEventsInformer.RaiseShutdown();
        }

        private void onShutdownCreated(IShutdown shutdown)
        {
            _globalEventsInformer.AddShutdown(shutdown);
        }

        public OrlemContainer Clone()
        {
            var retv = new OrlemContainer(_loggingService, _dynamicAssemblyName);
            retv._dependenciesSources.AddRange(_dependenciesSources);
            retv.Start();
            return retv;
        }
    }
}