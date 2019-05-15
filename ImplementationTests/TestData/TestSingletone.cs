using OrlemSoftware.Basics.Core.Attributes;

namespace ImplementationTests.TestData
{
    [Singletone]
    class TestSingletone : ITestSingletone
    {
        public int TestProperty { get; set; }
    }
}