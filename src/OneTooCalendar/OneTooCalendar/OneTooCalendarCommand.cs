using System;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class OneTooCalendarCommand : ICommand
	{
		private readonly Action<object?> _executeDelegate;

		public OneTooCalendarCommand(
			Action<object?> executeDelegate
			)
		{
			_executeDelegate = executeDelegate;
		}

		public event EventHandler? CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public bool CanExecute(object? parameter) => true;

		public void Execute(object? parameter) => _executeDelegate(parameter);
	}
}