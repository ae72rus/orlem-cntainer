using OrlemSoftware.Basics.Core;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Core.Implementation
{
    public class WpfCoreImplementationDependenciesSource : IDependenciesSource
    {
        public void SetDependencies(IContainer container)
        {
            container.Register<IRelayCommandFactory, RelayCommandFactory>();
            container.Register<IUiDispatcherService, UiDispatcherService>();
        }
    }
}