using System;
using System.Windows.Input;

namespace CGImageFiltering.App.ViewModels.Commands.Menu;

public class OpenFileCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        // TODO: implement open file dialog and image loading.
    }

    public event EventHandler? CanExecuteChanged;
}
