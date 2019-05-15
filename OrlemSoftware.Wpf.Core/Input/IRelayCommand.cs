using System.Windows.Input;

namespace OrlemSoftware.Wpf.Core.Input
{
    public interface IRelayCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}