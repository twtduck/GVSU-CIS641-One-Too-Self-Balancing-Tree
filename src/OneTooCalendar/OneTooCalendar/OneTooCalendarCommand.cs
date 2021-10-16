using System;
using System.Windows.Input;

namespace OneTooCalendar
{
    public class OneTooCalendarCommand : ICommand
    {
        private readonly Func<object?, bool> _canExecuteDelegate;
        private readonly Action<object?> _executeDelegate;

        public OneTooCalendarCommand(
            Action<object?> executeDelegate,
            Func<object?, bool>? canExecuteDelegate = default
            )
        {
            _canExecuteDelegate = canExecuteDelegate ?? (_ => true);
            _executeDelegate = executeDelegate;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecuteDelegate(parameter);

        public void Execute(object? parameter) => _executeDelegate(parameter);
    }
}