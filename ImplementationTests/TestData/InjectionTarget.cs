namespace ImplementationTests.TestData
{
    class InjectionTarget : IInjectionTarget
    {
        public ITestSingletone InjectedService { get; }

        public InjectionTarget(ITestSingletone testService)
        {
            InjectedService = testService;
        }
    }
}