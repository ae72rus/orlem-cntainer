using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrlemSoftware.Basics.Core;
using OrlemSoftware.Basics.Core.Implementation;
using OrlemSoftware.Core.Implementation.ASPNet;

namespace AspNetTests
{
    [TestClass]
    public class AspNetTestClass
    {
        [TestMethod]
        public void ResolveTest()
        {
            var resolver = new OrlemDependencyResolver(new MockLoggingService(), null);
            resolver.Container.AddDependenciesSource<DependenciesSource>();
            resolver.Container.Start();

            var service = resolver.GetService(typeof(IGenericInterface<int>));

            Assert.IsNotNull(service);
            var services = resolver.GetServices(typeof(IGenericInterface<string>));
            Assert.IsTrue(services.Any());

            var scopedResolver = resolver.BeginScope();

            var serviceFromScope = scopedResolver.GetService(typeof(IGenericInterface<int>));

            Assert.IsNotNull(serviceFromScope);
            var servicesFromScope = scopedResolver.GetServices(typeof(IGenericInterface<string>));
            Assert.IsTrue(servicesFromScope.Any());
        }
    }

    interface IGenericInterface<out T>
    {
        T Message { get; }
    }

    class GenericClass<T>: IGenericInterface<T>
    {
        public T Message { get; }

        public GenericClass()
        {
            Message = default(T);
        }
    }

    class DependenciesSource : IDependenciesSource
    {
        public void SetDependencies(IContainer container)
        {
            container.Register(typeof(IGenericInterface<>), typeof(GenericClass<>));
        }
    }
}
