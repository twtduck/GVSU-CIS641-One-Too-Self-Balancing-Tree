using System;
using System.Globalization;

namespace OneTooCalendar
{
	public delegate void ApplyEditsAndRefresh(IEventDataModel eventDataModel);
	
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
			var applyEditsAndRefresh = new ApplyEditsAndRefresh(
				eventDataModel =>
				{
					_eventsApi.UpdateEvent(eventDataModel);
					_calendarWeekViewModel!.StartRefreshEventsForWeekFromCache(_eventsApi, () => { });
				}
				);
			_calendarWeekViewModel = new CalendarWeekViewModel(
				GetFirstDayOfCurrentWeek(),
				eventCommandFactory,
				applyEditsAndRefresh
				);
			UpdateCurrentMonthAndYear();
			_backlogViewModel = new BacklogViewModel();
			PreviousWeekButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel =
					new CalendarWeekViewModel(
						CalendarWeekViewModel.StartDate.AddDays(-CalendarWeekViewModel.DaysInAWeek),
						eventCommandFactory,
						applyEditsAndRefresh
						)
				);
			NextWeekButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel =
					new CalendarWeekViewModel(
						CalendarWeekViewModel.StartDate.AddDays(CalendarWeekViewModel.DaysInAWeek),
						eventCommandFactory,
						applyEditsAndRefresh
						)
				);
			TodayButtonCommand = new OneTooCalendarCommand(
				_ => CalendarWeekViewModel = new CalendarWeekViewModel(GetFirstDayOfCurrentWeek(), eventCommandFactory, applyEditsAndRefresh)
				);
			RefreshButtonCommand = new OneTooCalendarCommand(_ =>
					OnRefreshButtonClicked()
				);

			DragDropHelper.InitializeCalendar(this);

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