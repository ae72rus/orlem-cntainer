using System.Collections.ObjectModel;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation.ViewModels
{
    public class BaseAutoViewModel<TModel, TPropertyDescriptorSource> : BaseViewModel, IAutoViewModel<TModel, TPropertyDescriptorSource>
        where TModel : IIdentifiable
        where TPropertyDescriptorSource : IPropertyDescriptorSource<TModel>
    {
        public ObservableCollection<IPropertyDescriptor<TModel>> PropertyDescriptors { get; }

        public BaseAutoViewModel(IUiDispatcherService dispatcherService,
            TModel model,
            IDescriptorSourceFactory<TModel> descriptorSourceFactory)
            : base(dispatcherService)
        {
            var source = descriptorSourceFactory.Create(model);
            var descriptors = source.GetDescriptors();
            PropertyDescriptors = new ObservableCollection<IPropertyDescriptor<TModel>>(descriptors);
        }

        public void SaveChanges()
        {
            foreach (var propertyDescriptor in PropertyDescriptors)
            {
                propertyDescriptor.SaveChanges();
            }
        }

        public void CancelChanges()
        {
            foreach (var propertyDescriptor in PropertyDescriptors)
            {
                propertyDescriptor.CancelChanges();
            }
        }

        public void Dispose()
        {

        }
    }
}