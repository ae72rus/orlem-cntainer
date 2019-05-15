using OrlemSoftware.Basics.Core;

namespace ImplementationTests.TestData
{
    public class ClassImpl : IClass
    {
        public int A { get; }
        public int B { get; }
        public string C { get; }

        public IRegionManager RegionManager { get; }

        public ClassImpl(IRegionManager regionManager, int a, int b)
            : this(regionManager)
        {
            A = a;
            B = b;
        }

        public ClassImpl(IRegionManager regionManager, string c)
            : this(regionManager)
        {
            C = c;
        }

        private ClassImpl(IRegionManager regionManager)
        {
            RegionManager = regionManager;
        }
    }
}