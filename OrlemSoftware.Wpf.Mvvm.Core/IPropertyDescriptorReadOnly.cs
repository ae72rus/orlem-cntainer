using System.ComponentModel;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IPropertyDescriptorReadOnly<out TModel> : INotifyPropertyChanged
        where TModel : IIdentifiable
    {
        TModel Model { get; }
        object Value { get; }
        string Label { get; }
    }
}