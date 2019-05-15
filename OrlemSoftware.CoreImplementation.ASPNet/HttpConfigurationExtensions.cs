using System;
using System.Web.Http;
using OrlemSoftware.Basics.Core;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Core.Implementation.ASPNet
{
    public static class HttpConfigurationExtensions
    {
        public static void UseOrlemDependencyResolver(this HttpConfiguration config, ILoggingService loggingService, Action<IContainer> containerSetup, string dynamicAssemblyName = null)
        {
            var resolver = new OrlemDependencyResolver(loggingService, dynamicAssemblyName);
            containerSetup?.Invoke(resolver.Container);
            resolver.Container.Start();
            config.DependencyResolver = resolver;
        }
    }
}