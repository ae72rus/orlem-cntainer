namespace ImplementationTests.TestData
{
    class CustomParametersTarget2 : CustomParametersTarget
    {
        public int IntFromParam2 { get; set; }

        public CustomParametersTarget2(ITestSingletone testSingletone, string stringParam, int intParam, int intParam2)
            : base(testSingletone, stringParam, intParam)
        {
            IntFromParam2 = intParam2;
        }
    }
}