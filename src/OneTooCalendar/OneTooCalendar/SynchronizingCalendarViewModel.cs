using System;
using System.Diagnostics;
using System.Windows.Input;

namespace OneTooCalendar
{
	public class SynchronizingCalendarViewModel : ViewModelBase { }
	public class InitializationErrorViewModel : ViewModelBase
	{
		public InitializationErrorViewModel(Action restartInitialization)
		{
			RestartInitializationCommand = new OneTooCalendarCommand(_ => restartInitialization());
		}

		public ICommand RestartInitializationCommand { get; }
	}
}