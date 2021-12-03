using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OneTooCalendar
{
	public class EventCommandFactory
	{
		private readonly IEventsApi _eventsApi;
		private readonly ICalendarsApi _calendarsApi;
		private readonly CalendarViewModel _calendarViewModel;

		public EventCommandFactory(IEventsApi eventsApi, ICalendarsApi calendarsApi, CalendarViewModel calendarViewModel)
		{
			_eventsApi = eventsApi;
			_calendarsApi = calendarsApi;
			_calendarViewModel = calendarViewModel;
		}
		public ICommand CreateDeleteEventCommand(IEventDataModel eventDataModel)
		{
			return new OneTooCalendarCommand(_ => DeleteEvent(eventDataModel));
		}

		private void DeleteEvent(IEventDataModel eventDataModel)
		{
			App.AssertUIThread();
			var restoreAction = _calendarViewModel.SetMainViewTemporarily!.Invoke(new SynchronizingCalendarViewModel());
			_eventsApi.DeleteEvent(eventDataModel);
			_calendarViewModel.CalendarWeekViewModel.StartRefreshEventsForWeekFromCache(_eventsApi, restoreAction);
		}

		public ICommand CreateEditEventCommand(IEventDataModel eventDataModel)
		{
			return new OneTooCalendarCommand(_ => EditEvent(eventDataModel));
		}


		private void EditEvent(IEventDataModel eventDataModel)
		{
			var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
			_calendarsApi.TryGetCalendarsAsync(token).RunCatchingFailure()
				.ContinueWith(t => EditEvent(eventDataModel, t.Result), default(CancellationToken));
		}

		private void EditEvent(IEventDataModel eventDataModel, ICalendarDataModel[]? googleCalendarInfos)
		{
			if (googleCalendarInfos is null)
				// TODO show error
				return;

			var eventDetailsViewModel = new EventDetailsViewModel(
				eventDataModel,
				googleCalendarInfos,
				_eventsApi,
				afterRefresh => _calendarViewModel.CalendarWeekViewModel.StartRefreshEventsForWeekFromCache(_eventsApi, afterRefresh)
				);
			var restoreAction = _calendarViewModel.SetMainViewTemporarily!.Invoke(eventDetailsViewModel);
			eventDetailsViewModel.RestoreAction = restoreAction;
		}

		public static StackPanel CreateEditEventControlContent()
		{
			return new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Children =
				{
					new Border
					{
						Child = new Path
						{
							Fill = Brushes.Black,
							Data = Geometry.Parse(
								"M20.71,7.04C21.1,6.65 21.1,6 20.71,5.63L18.37,3.29C18,2.9 17.35,2.9 16.96,3.29L15.12,5.12L18.87,8.87M3,17.25V21H6.75L17.81,9.93L14.06,6.18L3,17.25Z"
								),
						}
					},
					new TextBlock { Text = "Edit Event", Margin = new Thickness(8, 4, 4, 4) }
				}
			};
		}

		public static StackPanel CreateDeleteEventControlContent()
		{
			return new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Children =
				{
					new Border
					{
						Child = new Path
						{
							Fill = Brushes.Black,
							Data = Geometry.Parse(
								"M19,4H15.5L14.5,3H9.5L8.5,4H5V6H19M6,19A2,2 0 0,0 8,21H16A2,2 0 0,0 18,19V7H6V19Z"
								),
						}
					},
					new TextBlock { Text = "Delete Event", Margin = new Thickness(8, 4, 4, 4) }
				}
			};
		}
	}
}