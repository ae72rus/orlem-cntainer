using System.Collections.ObjectModel;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation.ViewModels
{
    public class BaseAutoViewModelReadOnly<TModel, TPropertyDescriptorSource> : BaseViewModel, IAutoViewModelReadOnly<TModel, TPropertyDescriptorSource>
        where TModel : IIdentifiable
        where TPropertyDescriptorSource : IPropertyDescriptorSourceReadOnly<TModel>
    {
        public ObservableCollection<IPropertyDescriptorReadOnly<TModel>> PropertyDescriptors { get; }

        public BaseAutoViewModelReadOnly(IUiDispatcherService dispatcherService,
            TModel model,
            IDescriptorSourceReadOnlyFactory<TModel> descriptorSourceFactory)
            : base(dispatcherService)
        {
            var source = descriptorSourceFactory.Create(model);
            var descriptors = source.GetDescriptors();
            PropertyDescriptors = new ObservableCollection<IPropertyDescriptorReadOnly<TModel>>(descriptors);
        }

        public void Dispose()
        {

        }
    }
}