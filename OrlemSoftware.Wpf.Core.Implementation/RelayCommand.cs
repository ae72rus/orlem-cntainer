using System;
using OrlemSoftware.Wpf.Core.Input;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Core.Implementation
{
    class RelayCommand : IRelayCommand
    {
        private readonly IUiDispatcherService _dispatcherService;

        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;

        private RelayCommand(IUiDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        public RelayCommand(IUiDispatcherService dispatcherService, Action action)
            : this(dispatcherService, x => action?.Invoke(), x => true)
        {

        }

        public RelayCommand(IUiDispatcherService dispatcherService, Action action, Func<bool> canExecute)
            : this(dispatcherService, x => action?.Invoke(), x => canExecute?.Invoke() ?? true)
        {

        }

        public RelayCommand(IUiDispatcherService dispatcherService, Action action, Func<object, bool> canExecute)
            : this(dispatcherService, x => action?.Invoke(), canExecute)
        {

        }

        public RelayCommand(IUiDispatcherService dispatcherService, Action<object> action)
            : this(dispatcherService, action, x => true)
        {

        }

        public RelayCommand(IUiDispatcherService dispatcherService, Action<object> action, Func<bool> canExecute)
            : this(dispatcherService, action, x => canExecute?.Invoke() ?? true)
        {

        }

        public RelayCommand(IUiDispatcherService dispatcherService, Action<object> action, Func<object, bool> canExecute)
            : this(dispatcherService)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            _dispatcherService?.UiDispatcher?.Invoke(() =>
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
