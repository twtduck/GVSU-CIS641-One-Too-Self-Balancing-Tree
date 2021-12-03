using System;
using System.Globalization;

namespace OneTooCalendar
{
	public class CalendarViewModel : ViewModelBase, IDisposable
	{
		private readonly IEventsApi _eventsApi;
		private CalendarWeekViewModel _calendarWeekViewModel;
		private BacklogViewModel _backlogViewModel;
		private string _currentMonthAndYear = string.Empty;

		public CalendarViewModel(IEventsApi eventsApi, ICalendarsApi calendarsApi)
		{
			_eventsApi = eventsApi;
			var eventCommandFactory = new EventCommandFactory(eventsApi, calendarsApi, this);
			_calendarWeekViewModel = new CalendarWeekViewModel(
				GetFirstDayOfCurrentWeek(),
				eventCommandFactory
				);
			UpdateCurrentMonthAndYear();
			_backlogViewModel = new BacklogViewModel();
			PreviousWeekButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel =
					new CalendarWeekViewModel(
						CalendarWeekViewModel.StartDate.AddDays(-CalendarWeekViewModel.DaysInAWeek),
						eventCommandFactory
						)
				);
			NextWeekButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel =
					new CalendarWeekViewModel(
						CalendarWeekViewModel.StartDate.AddDays(CalendarWeekViewModel.DaysInAWeek),
						eventCommandFactory
						)
				);
			TodayButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek(), eventCommandFactory)
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
			var restore = SetMainViewTemporarily!.Invoke(new SynchronizingCalendarViewModel());
			_calendarWeekViewModel.StartClearCachesAndUpdateEvents(_eventsApi, restore);
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
				var restoreCalendar = SetMainViewTemporarily!(new SynchronizingCalendarViewModel());
				_calendarWeekViewModel.Dispose();
				_calendarWeekViewModel = value;
				UpdateCurrentMonthAndYear();
				OnPropertyChanged();
				CalendarWeekViewModel.StartRefreshEventsForWeekFromCache(_eventsApi, restoreCalendar);
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

		public SetMainViewAndReturnRestoreAction? SetMainViewTemporarily { get; set; }

		public void Dispose()
		{
			_calendarWeekViewModel.Dispose();
		}
	}

	public delegate Action SetMainViewAndReturnRestoreAction(ViewModelBase viewModel);
}