namespace OrlemSoftware.Basics.Core
{
    public interface IRegionRegistrator
    {
        void Register<TInterface, TImplementation, TRegion>()
            where TImplementation : TInterface
            where TRegion : IRegion;

        void Register<TInterface, TRegion>(TInterface instance)
            where TRegion : IRegion;
        void RegisterFactory<TFactory, TInterface, TImplementation, TRegion>()
            where TFactory : IFactory
            where TImplementation : TInterface
            where TRegion : IRegion;
    }
}