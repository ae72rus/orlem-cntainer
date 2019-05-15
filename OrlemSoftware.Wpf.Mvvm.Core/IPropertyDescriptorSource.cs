using System.Collections.Generic;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IPropertyDescriptorSource<out TModel> : IPropertyDescriptorSourceReadOnly<TModel>
        where TModel : IIdentifiable
    {
        new IReadOnlyCollection<IPropertyDescriptor<TModel>> GetDescriptors();
    }
}