namespace ImplementationTests.TestData
{
    class CustomParametersTarget
    {
        public ITestSingletone InjectedService { get; }
        public string StringFromParam { get; }
        public int IntFromParam { get; }

        public CustomParametersTarget(ITestSingletone testSingletone, string stringParam, int intParam)
        {
            InjectedService = testSingletone;
            StringFromParam = stringParam;
            IntFromParam = intParam;
        }
    }
}