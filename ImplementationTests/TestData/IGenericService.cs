namespace ImplementationTests.TestData
{
    interface IGenericService<T>
    {
        T GenericProperty { get; set; }
    }
}