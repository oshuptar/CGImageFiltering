using System;
using System.Windows.Input;

namespace CGImageFiltering.App.ViewModels.Commands.Menu;

public class ResetFileCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        // TODO: implement reset to original image.
    }

    public event EventHandler? CanExecuteChanged;
}
