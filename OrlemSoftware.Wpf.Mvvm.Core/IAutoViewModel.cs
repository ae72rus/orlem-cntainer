using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IAutoViewModel<TModel, TPropertyDescriptorSource> : INotifyPropertyChanged, IDisposable
        where TModel : IIdentifiable
        where TPropertyDescriptorSource : IPropertyDescriptorSource<TModel>
    {
        ObservableCollection<IPropertyDescriptor<TModel>> PropertyDescriptors { get; }
        void SaveChanges();
        void CancelChanges();
    }
}