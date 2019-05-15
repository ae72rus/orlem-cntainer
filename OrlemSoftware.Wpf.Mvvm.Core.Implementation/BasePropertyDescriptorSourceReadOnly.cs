using System.Collections.Generic;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation
{
    public abstract class BasePropertyDescriptorSourceReadOnly<TModel> : IPropertyDescriptorSourceReadOnly<TModel>
        where TModel : IIdentifiable
    {
        public IReadOnlyCollection<IPropertyDescriptorReadOnly<TModel>> GetDescriptors()
        {
            return GetDescriptorsInternal();
        }

        protected abstract IReadOnlyCollection<IPropertyDescriptorReadOnly<TModel>> GetDescriptorsInternal();
    }
}