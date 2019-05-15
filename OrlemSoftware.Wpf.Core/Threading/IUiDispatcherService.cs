using System.Windows.Threading;

namespace OrlemSoftware.Wpf.Core.Threading
{
    public interface IUiDispatcherService
    {
        Dispatcher UiDispatcher { get; }
    }
}