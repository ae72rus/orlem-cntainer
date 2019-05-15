using System;
using OrlemSoftware.Basics.Core.Attributes;
using OrlemSoftware.Wpf.Core.Input;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Core.Implementation
{
    [Singletone]
    class RelayCommandFactory : IRelayCommandFactory
    {
        [Injectable]
        private IUiDispatcherService _dispatcherService;

        public IRelayCommand Create(Action action)
        {
            return new RelayCommand(_dispatcherService, action);
        }

        public IRelayCommand Create(Action action, Func<bool> canExecute)
        {
            return new RelayCommand(_dispatcherService, action, canExecute);
        }

        public IRelayCommand Create(Action action, Func<object, bool> canExecute)
        {
            return new RelayCommand(_dispatcherService, action, canExecute);
        }

        public IRelayCommand Create(Action<object> action)
        {
            return new RelayCommand(_dispatcherService, action);
        }

        public IRelayCommand Create(Action<object> action, Func<bool> canExecute)
        {
            return new RelayCommand(_dispatcherService, action, canExecute);
        }

        public IRelayCommand Create(Action<object> action, Func<object, bool> canExecute)
        {
            return new RelayCommand(_dispatcherService, action, canExecute);
        }
    }
}