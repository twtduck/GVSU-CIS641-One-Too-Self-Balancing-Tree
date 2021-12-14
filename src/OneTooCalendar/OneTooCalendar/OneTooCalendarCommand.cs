using System;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class OneTooCalendarCommand : ICommand
	{
		private readonly Action<object?> _executeDelegate;
		private readonly bool _enabled;

		public OneTooCalendarCommand(
			Action<object?> executeDelegate,
			bool enabled = true
			)
		{
			_executeDelegate = executeDelegate;
			_enabled = enabled;
		}

		public event EventHandler? CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public bool CanExecute(object? parameter) => _enabled;

		public void Execute(object? parameter) => _executeDelegate(parameter);
	}
}