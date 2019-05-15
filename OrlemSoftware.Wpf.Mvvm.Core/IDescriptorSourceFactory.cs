namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IDescriptorSourceFactory<TModel>
        where TModel : IIdentifiable
    {
        IPropertyDescriptorSource<TModel> Create(TModel model);
    }
}