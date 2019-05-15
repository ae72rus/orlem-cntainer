using OrlemSoftware.Basics.Core;

namespace ImplementationTests.TestData
{
    public interface IClass
    {
        int A { get; }
        int B { get; }
        string C { get; }
        IRegionManager RegionManager { get; }
    }

    public interface IClass2
    {
        IRegionManager RegionManager { get; }
    }
}