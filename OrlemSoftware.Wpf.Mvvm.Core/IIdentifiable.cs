using System;

namespace OrlemSoftware.Wpf.Mvvm.Core
{
    public interface IIdentifiable
    {
        object Id { get; }
        event EventHandler<EventArgs> Updated;
    }

    public interface IIdentifiable<out T> : IIdentifiable
    {
        new T Id { get; }
    }
}
