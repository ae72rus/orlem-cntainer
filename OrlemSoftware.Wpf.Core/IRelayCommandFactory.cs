using System;
using OrlemSoftware.Wpf.Core.Input;

namespace OrlemSoftware.Wpf.Core
{
    public interface IRelayCommandFactory
    {
        IRelayCommand Create(Action action);
        IRelayCommand Create(Action action, Func<bool> canExecute);
        IRelayCommand Create(Action action, Func<object, bool> canExecute);

        IRelayCommand Create(Action<object> action);
        IRelayCommand Create(Action<object> action, Func<bool> canExecute);
        IRelayCommand Create(Action<object> action, Func<object, bool> canExecute);
    }
}
