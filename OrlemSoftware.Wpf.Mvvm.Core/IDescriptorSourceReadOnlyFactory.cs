namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IDescriptorSourceReadOnlyFactory<TModel>
        where TModel : IIdentifiable
    {
        IPropertyDescriptorSourceReadOnly<TModel> Create(TModel model);
    }
}