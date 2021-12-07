using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace OneTooCalendar
{
	public enum EventMovementType
	{
		OnCalendar,
		ToBacklog,
		FromBacklog
	}
	public delegate void ApplyEditsAndRefresh(IEventDataModel eventDataModel, EventMovementType eventMovementType, Action afterRefreshAction);

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
				(eventDataModel, eventMovementType, afterRefreshAction) =>
				{
					switch (eventMovementType)
					{
						case EventMovementType.OnCalendar:
							_eventsApi.UpdateEvent(eventDataModel);
							break;
						case EventMovementType.ToBacklog:
							var recreatedToBacklog = GoogleCalendarEventDataModel.RecreateId((GoogleCalendarEventDataModel)eventDataModel);
							_eventsApi.DeleteEvent(eventDataModel);
							_backlogViewModel.BacklogEvents.Add(new BacklogEventViewModel(recreatedToBacklog));
							break;
						case EventMovementType.FromBacklog:
							var recreatedFromBacklog = GoogleCalendarEventDataModel.RecreateId((GoogleCalendarEventDataModel)eventDataModel);
							_eventsApi.AddEvent(recreatedFromBacklog);
							foreach (var matchingEvent in _backlogViewModel.BacklogEvents.Where(x => x.EventDataModel == eventDataModel).Distinct().ToList())
							{
								_backlogViewModel.BacklogEvents.Remove(matchingEvent);
								Debug.Assert(!_backlogViewModel.BacklogEvents.Any(x => x.EventDataModel == eventDataModel));
							}
							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(eventMovementType), eventMovementType, null);
					}
					_calendarWeekViewModel!.StartRefreshEventsForWeekFromCache(_eventsApi, afterRefreshAction);
				}
				);
			_calendarWeekViewModel = new CalendarWeekViewModel(
				GetFirstDayOfCurrentWeek(),
				eventCommandFactory,
				applyEditsAndRefresh
				);
			UpdateCurrentMonthAndYear();
			_backlogViewModel = new BacklogViewModel(applyEditsAndRefresh);
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