using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation.ViewModels
{
    public abstract class BaseModelRepository<TModel> : IModelRepository<TModel>
        where TModel : IIdentifiable
    {
        private bool _modelsAreLoaded;
        private int _subscribersCount;
        private List<TModel> _models = new List<TModel>();
        public IReadOnlyCollection<TModel> Models => _models;
        public event EventHandler<ModelEventArgs<TModel>> ModelCreated;
        public event EventHandler<ModelEventArgs<TModel>> ModelUpdated;
        public event EventHandler<ModelEventArgs<TModel>> ModelDeleted;

        public void AddSubscriber()
        {
            _subscribersCount++;
        }

        public void RemoveSubscriber()
        {
            _subscribersCount--;
            if (_subscribersCount <= 0)
                Dispose();
        }

        public async Task LoadModels()
        {
            if(_modelsAreLoaded)
                return;

            _models = (await GetModels())?.ToList() ?? new List<TModel>();
            _modelsAreLoaded = _models.Any();
        }
        
        public void Dispose()
        {
            DisposeInternal();
        }

        protected void OnModelCreated(object modelId)
        {
            var model = LoadModel(modelId);
            _models.Add(model);
            ModelCreated?.Invoke(this, new ModelEventArgs<TModel>(model));
        }

        protected void OnModelDeleted(object modelId)
        {
            var model = _models.FirstOrDefault(x => x.Id.Equals(modelId));
            if (model == null)
                return;

            _models.Remove(model);
            ModelDeleted?.Invoke(this, new ModelEventArgs<TModel>(model));
        }

        protected void OnModelUpdated(object modelId)
        {
            var loadedModel = LoadModel(modelId);
            var localModel = _models.FirstOrDefault(x => x.Equals(modelId));

            if (localModel == null || loadedModel == null)
                return;

            SyncronizeModels(localModel, loadedModel);
            ModelUpdated?.Invoke(this, new ModelEventArgs<TModel>(localModel));
        }

        protected abstract void DisposeInternal();
        protected abstract TModel LoadModel(object id);
        protected abstract Task<IReadOnlyCollection<TModel>> GetModels();
        protected abstract void SyncronizeModels(TModel localModel, TModel loadedModel);
    }
}