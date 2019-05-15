using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Threading;
using OrlemSoftware.Wpf.Core;
using OrlemSoftware.Wpf.Core.Input;
using OrlemSoftware.Wpf.Core.Threading;
using OrlemSoftware.Wpf.Mvvm.Core;
using OrlemSoftware.Wpf.Mvvm.Core.Implementation;
using OrlemSoftware.Wpf.Mvvm.Core.Implementation.ViewModels;

namespace WpfTestApp
{
    class MainViewModel
    {
        private Model _model;
        public AutoViewModel AutoViewModel { get; }
        public ICommand SetTextCommand { get; }
        public ICommand SaveChangesCommand { get; }
        public MainViewModel()
        {
            SetTextCommand = new RelayCommand(setTextCommand);
            SaveChangesCommand = new RelayCommand(saveChanges);
            _model = new Model();
            AutoViewModel = new AutoViewModel(_model);
        }

        private void setTextCommand()
        {
            _model.StringModelValue = DateTime.Now.ToString();
        }

        private void saveChanges()
        {
            AutoViewModel?.SaveChanges();
        }
    }

    class RelayCommand : IRelayCommand
    {
        private Action _action;
        public RelayCommand(Action action)
        {
            _action = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    class Model : IIdentifiable<int>
    {
        private static int _id = 0;
        private string _stringModelValue;
        private int _intModelValue;

        public string StringModelValue
        {
            get => _stringModelValue;
            set
            {
                _stringModelValue = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        public int IntModelValue
        {
            get => _intModelValue;
            set
            {
                _intModelValue = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }

        object IIdentifiable.Id => Id;

        public int Id { get; } = _id++;
        public event EventHandler<EventArgs> Updated;
    }

    class DispatcherService : IUiDispatcherService
    {
        public static DispatcherService Instance => _instance1 ?? (_instance1 = new DispatcherService());

        private Dispatcher _uiDispatcher;
        private static DispatcherService _instance1;

        public Dispatcher UiDispatcher => _uiDispatcher ?? (_uiDispatcher = Dispatcher.CurrentDispatcher);
    }

    class AutoViewModel : BaseAutoViewModel<Model, PropertyDescriptorSource>
    {
        public AutoViewModel(Model model)
            : base(DispatcherService.Instance, model, ModelDescriptorSourceFactory.Instance)
        {

        }
    }

    class PropertyDescriptorSource : BasePropertyDescriptorSource<Model>
    {
        public PropertyDescriptorSource(Model model) : base(model)
        {
        }

        protected override IReadOnlyCollection<IPropertyDescriptor<Model>> GetDescriptorsInternal(Model model)
        {
            return new List<IPropertyDescriptor<Model>>
            {
                new PropertyDescriptor<Model>(DispatcherService.Instance, model, m => m.StringModelValue, (o, m) =>
                {
                    m.StringModelValue = (string)o;
                }, "String Property"),
                new PropertyDescriptor<Model>(DispatcherService.Instance, model, m => m.IntModelValue, (o, m) =>
                {
                    m.IntModelValue = (int)o;
                }, "Integer Property"),
            };
        }
    }


    class ModelDescriptorSourceFactory : IDescriptorSourceFactory<Model>
    {
        private static ModelDescriptorSourceFactory _instance;

        public static ModelDescriptorSourceFactory Instance => _instance ?? (_instance = new ModelDescriptorSourceFactory());

        public IPropertyDescriptorSource<Model> Create(Model model)
        {
            return new PropertyDescriptorSource(model);
        }
    }
}