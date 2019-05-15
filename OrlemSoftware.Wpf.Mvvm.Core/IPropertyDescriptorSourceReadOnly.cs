using System.Collections.Generic;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IPropertyDescriptorSourceReadOnly<out TModel>
        where TModel : IIdentifiable
    {
        IReadOnlyCollection<IPropertyDescriptorReadOnly<TModel>> GetDescriptors();
    }
}