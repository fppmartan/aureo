using System.Windows.Input;

namespace Aureo.ViewModels
{
    /// <summary>
    /// Provides an ICommand implementation that executes an action and optionally determines whether execution is
    /// allowed.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // The action to execute when the command is invoked.
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        // Initializes a new instance of the RelayCommand class with the specified execute action and optional canExecute predicate.
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) 
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Event that is raised when the ability of the command to execute changes, allowing the UI to update accordingly.
        public event EventHandler? CanExecuteChanged 
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Determines whether the command can execute by invoking the canExecute predicate if provided, or returning true if not.
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        // Executes the command by invoking the execute action with the provided parameter.
        public void Execute(object? parameter) => _execute(parameter);
    }
}
