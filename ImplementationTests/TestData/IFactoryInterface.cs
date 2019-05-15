using OrlemSoftware.Basics.Core;

namespace ImplementationTests.TestData
{
    public interface IFactoryInterface : IFactory
    {
        IClass Create(int a, int b);
        IClass Create(string c);
        IClass Create();
    }
}