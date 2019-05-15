using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using OrlemSoftware.Basics.Core.Implementation;
using OrlemSoftware.Basics.Core.Logging;

namespace OrlemSoftware.Core.Implementation.ASPNet
{
    class OrlemDependencyResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        public OrlemContainer Container { get; }

        public OrlemDependencyResolver(ILoggingService loggingService, string dynamicAssemblyName) 
            : this(new OrlemContainer(loggingService, dynamicAssemblyName))
        {

        }

        private OrlemDependencyResolver(OrlemContainer orlemContainer)
        {
            Container = orlemContainer;
        }

        public void Dispose()
        {
            Container?.Stop();
        }

        public object GetService(Type serviceType)
        {
            return Container?.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var resolved = Container?.Resolve(serviceType);
            return resolved != null
                ? new[] { resolved } 
                : new object[0];
        }

        public IDependencyScope BeginScope()
        {
            return new OrlemDependencyResolver(Container?.Clone());
        }

    }
}