namespace ImplementationTests.TestData
{
    class GenericService<T> : IGenericService<T>
    {
        public T GenericProperty { get; set; }
    }
}