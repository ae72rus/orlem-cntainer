using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IModelRepository<TModel> : IDisposable
        where TModel : IIdentifiable
    {
        IReadOnlyCollection<TModel> Models { get; }
        event EventHandler<ModelEventArgs<TModel>> ModelCreated;
        event EventHandler<ModelEventArgs<TModel>> ModelUpdated;
        event EventHandler<ModelEventArgs<TModel>> ModelDeleted;
        void AddSubscriber();
        void RemoveSubscriber();
        Task LoadModels();
    }
}