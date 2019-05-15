using System;

namespace OrlemSoftware.Basics.Core.Implementation
{
    class Registrator : BaseContainer, IRegistrator
    {
        private readonly FactoryGenerator _factoryGenerator = new FactoryGenerator();
        private readonly string _dynamicAssemblyName;

        public Registrator(string dynamicAssemblyName)
        {
            _dynamicAssemblyName = dynamicAssemblyName;
        }

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Register(typeof(TInterface), typeof(TImplementation));
        }

        public void Register<TInterface>(TInterface instance)
        {
            Register(typeof(TInterface), instance);
        }

        public new void Register(Type abstraction, Type implementation)
        {
            base.Register(abstraction, implementation);
        }

        public void RegisterFactory<TFactory, TInterface, TImplementation>()
            where TFactory : IFactory
            where TImplementation : TInterface
        {
            var implType = _factoryGenerator.GenerateFactory<TFactory, TInterface, TImplementation>(_dynamicAssemblyName);
            Register(typeof(TFactory), implType);
        }
    }
}