using System;
using OrlemSoftware.Wpf.Core.Threading;
using OrlemSoftware.Wpf.Mvvm.Core.Implementation.ViewModels;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation
{
    public class PropertyDescriptor<TModel> : BaseViewModel, IPropertyDescriptor<TModel>
        where TModel : IIdentifiable
    {
        private bool _isSaving;
        private readonly Func<TModel, object> _getValueFunc;
        private readonly Action<object, TModel> _setValueAction;
        private object _value;

        public object Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public TModel Model { get; }

        public string Label { get; }

        object IPropertyDescriptorReadOnly<TModel>.Value => Value;

        public PropertyDescriptor(
            IUiDispatcherService dispatcherService,
            TModel model,
            Func<TModel, object> getValueFunc,
            Action<object, TModel> setValueAction,
            string label)
            : base(dispatcherService)
        {
            _getValueFunc = getValueFunc;
            _setValueAction = setValueAction;
            Model = model;
            Label = label;
            Model.Updated += onModelUpdated;
        }

        private void onModelUpdated(object sender, EventArgs e)
        {
            if(_isSaving)
                return;

            Value = _getValueFunc?.Invoke(Model);
        }

        public PropertyDescriptor(
            IUiDispatcherService dispatcherService,
            TModel model,
            Func<TModel, object> getValueFunc,
            string label)
            : this(
                dispatcherService,
                model,
                getValueFunc,
                (x, y) => { },
                label)
        {

        }

        public void CancelChanges()
        {
            Value = _getValueFunc?.Invoke(Model);
        }

        public void SaveChanges()
        {
            _isSaving = true;
               _setValueAction?.Invoke(Value, Model);
            _isSaving = false;
        }
    }
}