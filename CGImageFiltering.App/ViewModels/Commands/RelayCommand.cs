using System;
using System.Windows.Input;

namespace CGImageFiltering.App.ViewModels.Commands;

public class RelayCommand(Action<object?> execute, Func<object?, bool> canExecute) : ICommand
{
    public virtual bool CanExecute(object? parameter) => canExecute(parameter);

    public virtual void Execute(object? parameter) => execute(parameter);

    public event EventHandler? CanExecuteChanged;
    
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}