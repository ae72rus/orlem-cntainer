using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IAutoViewModelReadOnly<TModel, TPropertyDescriptorSource> : INotifyPropertyChanged, IDisposable
        where TModel : IIdentifiable
        where TPropertyDescriptorSource : IPropertyDescriptorSourceReadOnly<TModel>
    {
        ObservableCollection<IPropertyDescriptorReadOnly<TModel>> PropertyDescriptors { get; }
    }
}