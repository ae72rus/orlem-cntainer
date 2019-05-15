using System;
using System.Collections.Generic;

namespace OrlemSoftware.Basics.Core
{
    public interface IRegionManager : IRegionRegistrator, IRegionResolver
    {
        IReadOnlyCollection<Type> GetRegisteredTypes<TRegion>()
            where TRegion : IRegion;
    }
}