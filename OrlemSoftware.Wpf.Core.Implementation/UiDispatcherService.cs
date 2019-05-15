using System.Windows.Threading;
using OrlemSoftware.Basics.Core.Attributes;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Core.Implementation
{
    [Singletone]
    class UiDispatcherService : IUiDispatcherService
    {
        private Dispatcher _dispatcher;
        public Dispatcher UiDispatcher => _dispatcher 
                                          ?? (_dispatcher = Dispatcher.CurrentDispatcher);
    }
}