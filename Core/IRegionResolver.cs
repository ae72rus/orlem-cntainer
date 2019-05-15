using System;

namespace OrlemSoftware.Basics.Core
{
    public interface IRegionResolver
    {
        TInterface Resolve<TInterface, TRegion>()
            where TRegion : IRegion;
        TInterface Resolve<TInterface, TRegion>(params object[] args)
            where TRegion : IRegion;
        TInterface Resolve<TInterface, TRegion>(params ResolveParameter[] parameters)
            where TRegion : IRegion;
        object Resolve<TRegion>(Type type)
            where TRegion : IRegion;
        object Resolve<TRegion>(Type type, params object[] args)
            where TRegion : IRegion;
        object Resolve<TRegion>(Type type, params ResolveParameter[] parameters)
            where TRegion : IRegion;
    }
}