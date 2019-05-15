using System;

namespace OrlemSoftware.Basics.Core
{
    public interface IRegistrator
    {
        void Register<TInterface, TImplementation>() where TImplementation : TInterface;
        void Register<TInterface>(TInterface instance);
        void Register(Type abstraction, Type implementation);
        void RegisterFactory<TFactory, TInterface, TImplementation>()
            where TFactory : IFactory
            where TImplementation : TInterface;
    }
}