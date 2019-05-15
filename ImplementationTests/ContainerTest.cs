using System;
using System.Linq;
using ImplementationTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrlemSoftware.Basics.Core;
using OrlemSoftware.Basics.Core.Implementation;

namespace ImplementationTests
{
    [TestClass]
    public class ContainerTest
    {
        [TestMethod]
        public void ResolveTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestService, TestService>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestService>();

            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void SingletoneTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestSingletone, TestSingletone>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestSingletone>();
            Assert.IsNotNull(service);
            service.TestProperty = 99;

            var service2 = registerResolver.Resolve<ITestSingletone>();

            Assert.IsNotNull(service2);
            Assert.AreEqual(99, service2.TestProperty);
        }

        [TestMethod]
        public void InjectionTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestSingletone, TestSingletone>();
            registerResolver.Register<IInjectionTarget, InjectionTarget>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestSingletone>();
            Assert.IsNotNull(service);
            service.TestProperty = 99;

            var target = registerResolver.Resolve<IInjectionTarget>();

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.InjectedService);
            Assert.AreEqual(99, target.InjectedService.TestProperty);
        }

        [TestMethod]
        public void ResolveTestWithoutRegistration()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestSingletone, TestSingletone>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestSingletone>();
            Assert.IsNotNull(service);
            service.TestProperty = 99;

            var target = registerResolver.Resolve<InjectionTarget>();

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.InjectedService);
            Assert.AreEqual(99, target.InjectedService.TestProperty);
        }

        [TestMethod]
        public void ResolveWithParamsTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestSingletone, TestSingletone>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestSingletone>();
            Assert.IsNotNull(service);
            service.TestProperty = 99;

            var target = registerResolver.Resolve<CustomParametersTarget>("abc", 22);

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.InjectedService);
            Assert.AreEqual(99, target.InjectedService.TestProperty);
            Assert.AreEqual("abc", target.StringFromParam);
            Assert.AreEqual(22, target.IntFromParam);
        }

        [TestMethod]
        public void ResolveWithWrongOrderedParamsTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestSingletone, TestSingletone>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestSingletone>();
            Assert.IsNotNull(service);
            service.TestProperty = 99;

            var target = registerResolver.Resolve<CustomParametersTarget>(22, "abc");

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.InjectedService);
            Assert.AreEqual(99, target.InjectedService.TestProperty);
            Assert.AreEqual("abc", target.StringFromParam);
            Assert.AreEqual(22, target.IntFromParam);
        }

        [TestMethod]
        public void ResolveWithNamedParamsTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register<ITestSingletone, TestSingletone>();
            registerResolver.Start();
            var service = registerResolver.Resolve<ITestSingletone>();
            Assert.IsNotNull(service);
            service.TestProperty = 99;

            var target = registerResolver.Resolve<CustomParametersTarget2>(
                new ResolveParameter
                {
                    ParameterName = "stringParam",
                    Parameter = "qwerty"
                },
                new ResolveParameter
                {
                    ParameterName = "intParam2",
                    Parameter = 44
                },
                new ResolveParameter
                {
                    ParameterName = "intParam",
                    Parameter = 33
                });

            Assert.IsNotNull(target);
            Assert.IsNotNull(target.InjectedService);
            Assert.AreEqual(99, target.InjectedService.TestProperty);
            Assert.AreEqual("qwerty", target.StringFromParam);
            Assert.AreEqual(33, target.IntFromParam);
            Assert.AreEqual(44, target.IntFromParam2);
        }

        [TestMethod]
        public void GenericsTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.Register(typeof(IGenericService<>), typeof(GenericService<>));
            registerResolver.Start();
            var service1 = registerResolver.Resolve<IGenericService<string>>();
            var service2 = registerResolver.Resolve<IGenericService<int>>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);

            Assert.IsNull(service1.GenericProperty);
            Assert.AreEqual(0, service2.GenericProperty);
        }

        [TestMethod]
        public void DynamicFactoryTest()
        {
            var registerResolver = new OrlemContainer(new LoggingService());
            registerResolver.RegisterFactory<IFactoryInterface, IClass, ClassImpl>();
            registerResolver.Start();
            var factory = registerResolver.Resolve<IFactoryInterface>();

            Assert.IsNotNull(factory);

            var instance1 = factory.Create(0, 1);
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance1.RegionManager);
            Assert.AreEqual(0, instance1.A);
            Assert.AreEqual(1, instance1.B);
            Assert.IsNull(instance1.C);

            var instance2 = factory.Create("str");
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance2.RegionManager);
            Assert.AreEqual(0, instance2.A);
            Assert.AreEqual(0, instance2.B);
            Assert.AreEqual("str", instance2.C);
        }

        [TestMethod]
        public void DynamicFactoryTest2()
        {
            var dynamicAsmName = "TestDynamicAssembly";
            var registerResolver = new OrlemContainer(new LoggingService(), dynamicAsmName);
            registerResolver.Start();
            registerResolver.RegisterFactory<IFactoryInterface, IClass, ClassImpl>();
            registerResolver.RegisterFactory<IFactoryInterface2, IClass, ClassImpl>();
            var factory = registerResolver.Resolve<IFactoryInterface>();

            Assert.IsNotNull(factory);

            var instance1 = factory.Create(0, 1);
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance1.RegionManager);
            Assert.AreEqual(0, instance1.A);
            Assert.AreEqual(1, instance1.B);
            Assert.IsNull(instance1.C);

            var instance2 = factory.Create("str");
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance2.RegionManager);
            Assert.AreEqual(0, instance2.A);
            Assert.AreEqual(0, instance2.B);
            Assert.AreEqual("str", instance2.C);

            var factory2 = registerResolver.Resolve<IFactoryInterface2>();

            Assert.IsNotNull(factory2);

            var instance4 = factory2.Create(0, 1);
            Assert.IsNotNull(instance4);
            Assert.IsNotNull(instance4.RegionManager);
            Assert.AreEqual(0, instance4.A);
            Assert.AreEqual(1, instance4.B);
            Assert.IsNull(instance4.C);

            var instance5 = factory2.Create("str");
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance5.RegionManager);
            Assert.AreEqual(0, instance5.A);
            Assert.AreEqual(0, instance5.B);
            Assert.AreEqual("str", instance5.C);

            var asms = AppDomain.CurrentDomain.GetAssemblies().Where(x=>x.GetName().Name == dynamicAsmName).ToArray();
            Assert.AreEqual(1, asms.Length);
            var asm = asms.First();
            var modules = asm.Modules.ToArray();
            Assert.AreEqual(1, modules.Length);
            var module = modules.First();
            var types = module.GetTypes().ToArray();
            Assert.AreEqual(2, types.Length);
        }

        [TestMethod]
        public void DynamicFactoryTest3()
        {
            var dynamicAsmName = "TestDynamicAssembly";
            var registerResolver = new OrlemContainer(new LoggingService(), dynamicAsmName);
            registerResolver.Start();

            registerResolver.RegisterFactory<IClass2Factory, IClass2, Class2Impl>();
            var factory = registerResolver.Resolve<IClass2Factory>();
            var instance1 = factory.Create();
            Assert.IsNotNull(instance1?.RegionManager);
        }
    }
}
