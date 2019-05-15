using System.Collections.Generic;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation
{
    public abstract class BasePropertyDescriptorSource<TModel> : IPropertyDescriptorSource<TModel>
        where TModel : IIdentifiable
    {
        private readonly TModel _model;
        protected BasePropertyDescriptorSource(TModel model)
        {
            _model = model;
        }

        IReadOnlyCollection<IPropertyDescriptorReadOnly<TModel>> IPropertyDescriptorSourceReadOnly<TModel>.GetDescriptors()
        {
            return ((IPropertyDescriptorSource<TModel>)this).GetDescriptors();
        }

        IReadOnlyCollection<IPropertyDescriptor<TModel>> IPropertyDescriptorSource<TModel>.GetDescriptors()
        {
            return GetDescriptorsInternal(_model);
        }

        protected abstract IReadOnlyCollection<IPropertyDescriptor<TModel>> GetDescriptorsInternal(TModel model);
    }
}