using System;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public class ModelEventArgs<TModel> : EventArgs
        where TModel : IIdentifiable
    {
        public TModel Model { get; }

        public ModelEventArgs(TModel model)
        {
            Model = model;
        }
    }
}