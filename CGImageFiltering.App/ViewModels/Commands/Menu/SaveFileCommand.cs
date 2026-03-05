using System;
using System.Windows.Input;

namespace CGImageFiltering.App.ViewModels.Commands.Menu;

public class SaveFileCommand : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        // TODO: implement save filtered image.
    }

    public event EventHandler? CanExecuteChanged;
}
