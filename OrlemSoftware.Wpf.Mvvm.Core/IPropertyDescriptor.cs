namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IPropertyDescriptor<out TModel> : IPropertyDescriptorReadOnly<TModel>
        where TModel : IIdentifiable
    {
        new object Value { get; set; }
        void SaveChanges();
        void CancelChanges();
    }
}