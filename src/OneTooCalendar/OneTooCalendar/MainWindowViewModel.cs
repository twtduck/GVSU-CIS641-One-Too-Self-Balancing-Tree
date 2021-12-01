namespace OneTooCalendar
{
	public class MainWindowViewModel : ViewModelBase
	{
		private ViewModelBase _currentView = new SynchronizingCalendarViewModel();

		public ViewModelBase CurrentView
		{
			get => _currentView;
			set
			{
				_currentView = value;
				OnPropertyChanged();
			}
		}
	}
}