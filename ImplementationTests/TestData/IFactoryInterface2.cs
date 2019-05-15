using OrlemSoftware.Basics.Core;

namespace ImplementationTests.TestData
{
    public interface IFactoryInterface2 : IFactory
    {
        IClass Create(int a, int b);
        IClass Create(string c);
        IClass Create();
    }
}