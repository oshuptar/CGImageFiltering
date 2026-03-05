using System;
using System.Windows.Input;

namespace CGImageFiltering.App.ViewModels.Commands.Menu;

public class OpenFileCommand(Action<object?> execute, Func<object?, bool> canExecute) : ICommand
{
    public bool CanExecute(object? parameter) => canExecute(parameter);
    public void Execute(object? parameter) => execute(parameter);

    public event EventHandler? CanExecuteChanged;
}
