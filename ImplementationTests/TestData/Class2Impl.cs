using OrlemSoftware.Basics.Core;

namespace ImplementationTests.TestData
{
    public class Class2Impl : IClass2
    {
        public IRegionManager RegionManager { get; }

        public Class2Impl(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }
    }
}