
namespace OrlemSoftware.Basics.Core
{
    public interface IContainer : IRegistrator, IResolver, IRegionManager, IGlobalEventsInformer
    {
        IContainer AddDependenciesSource<TDependenciesSource>()
            where TDependenciesSource : class, IDependenciesSource, new();
    }
}
