using System;
using System.Globalization;
using System.Threading;

namespace OneTooCalendar
{
	public class CalendarViewModel : ViewModelBase, IDisposable
	{
		private readonly GoogleCalendarService _googleCalendarService;
		private CalendarWeekViewModel _calendarWeekViewModel;
		private BacklogViewModel _backlogViewModel;
		private string _currentMonthAndYear = string.Empty;

		public CalendarViewModel(GoogleCalendarService googleCalendarService)
		{
			_googleCalendarService = googleCalendarService;
			_calendarWeekViewModel = new CalendarWeekViewModel(
				GetFirstDayOfCurrentWeek(),
				_googleCalendarService,
				this
				);
			UpdateCurrentMonthAndYear();
			_backlogViewModel = new BacklogViewModel();
			PreviousWeekButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel =
					new CalendarWeekViewModel(
						CalendarWeekViewModel.StartDate.AddDays(-CalendarWeekViewModel.DaysInAWeek),
						_googleCalendarService,
						this
						)
				);
			NextWeekButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel =
					new CalendarWeekViewModel(
						CalendarWeekViewModel.StartDate.AddDays(CalendarWeekViewModel.DaysInAWeek),
						_googleCalendarService,
						this
						)
				);
			TodayButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek(), _googleCalendarService, this)
				);
			RefreshButtonCommand = new OneTooCalendarCommand(_ =>
					OnRefreshButtonClicked()
				);

			DateTime GetFirstDayOfCurrentWeek()
			{
				return DateTime.Today.Subtract(TimeSpan.FromDays((int)DateTime.Today.DayOfWeek));
			}
		}

		private void OnRefreshButtonClicked()
		{
			var restore = SetMainViewTemporarily(new SynchronizingCalendarViewModel());
			CalendarWeekViewModel.TryRefreshEventsAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token)
				.RunCatchingFailure().ContinueWith(_ => restore.Invoke()); // TODO handle completed task result
		}

		public OneTooCalendarCommand NextWeekButtonCommand { get; }

		public OneTooCalendarCommand PreviousWeekButtonCommand { get; }

		public OneTooCalendarCommand TodayButtonCommand { get; }
		public OneTooCalendarCommand RefreshButtonCommand { get; }

		public CalendarWeekViewModel CalendarWeekViewModel
		{
			get => _calendarWeekViewModel;
			set
			{
				var restoreCalendar = SetMainViewTemporarily(new SynchronizingCalendarViewModel());
				_calendarWeekViewModel.Dispose();
				_calendarWeekViewModel = value;
				UpdateCurrentMonthAndYear();
				OnPropertyChanged();
				CalendarWeekViewModel.TryRefreshEventsAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token)
					.RunCatchingFailure()
					.ContinueWith(_ => restoreCalendar.Invoke()); // TODO handle false
			}
		}

		private void UpdateCurrentMonthAndYear()
		{
			var date = CalendarWeekViewModel.StartDate;
			var months = CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames;
			CurrentMonthAndYear = $"{months[date.Month - 1]} {date.Year}";
		}

		public string CurrentMonthAndYear
		{
			get => _currentMonthAndYear;
			set
			{
				_currentMonthAndYear = value;
				OnPropertyChanged();
			}
		}

		public BacklogViewModel BacklogViewModel
		{
			get => _backlogViewModel;
			set
			{
				_backlogViewModel = value;
				OnPropertyChanged();
			}
		}

		public SetMainViewAndReturnRestoreAction SetMainViewTemporarily { get; set; }

		public void Dispose()
		{
			_calendarWeekViewModel.Dispose();
		}
	}
	
	public delegate Action SetMainViewAndReturnRestoreAction(ViewModelBase viewModel);
}