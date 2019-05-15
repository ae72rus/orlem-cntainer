using System;

namespace OrlemSoftware.Basics.Core
{
    public interface IResolver
    {
        TInterface Resolve<TInterface>();
        TInterface Resolve<TInterface>(params object[] args);
        TInterface Resolve<TInterface>(params ResolveParameter[] parameters);
        object Resolve(Type type);
        object Resolve(Type type, params object[] args);
        object Resolve(Type type, params ResolveParameter[] parameters);
    }
}