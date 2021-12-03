using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace OneTooCalendar
{
	public class CalendarWeekViewModel : ViewModelBase, IDisposable
	{
		public const int DaysInAWeek = 7;

		public CalendarWeekViewModel(
			DateTime startDate,
			EventCommandFactory eventCommandFactory
			)
		{
			for (var i = 0; i < DaysInAWeek; i++)
			{
				DateViewModels.Add(new DateViewModel(startDate + TimeSpan.FromDays(i), eventCommandFactory)
				{
					BorderOpacity = i == 0 ? 0 : 1
				});
			}

			StartDate = startDate;
		}

		public DateTime StartDate { get; }

		public List<DateViewModel> DateViewModels { get; } = new List<DateViewModel>();

		public List<HourLabelViewModel> TimeLabels =>
			Enumerable.Range(0, 24)
				.Select(hourNumber => new HourLabelViewModel(hourNumber))
				.ToList();

		public void StartClearCachesAndUpdateEvents(IEventsApi eventsApi, Action afterUpdate)
		{
			App.AssertUIThread();
			var dispatcher = Dispatcher.CurrentDispatcher;
			eventsApi.ClearCachesAndGetEventsForWeek(
				StartDate,
				events => dispatcher.Invoke(
					() =>
					{
						if (events is not null)
							UpdateFromEventsList(events);
						else // TODO
							Debug.Fail($"Unable to get events in {nameof(StartClearCachesAndUpdateEvents)}");
						afterUpdate();
					}
					)
				);
		}

		public void StartRefreshEventsForWeekFromCache(IEventsApi eventsApi, Action? afterRefresh)
		{
			App.AssertUIThread();
			var dispatcher = Dispatcher.CurrentDispatcher;
			var token = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
			eventsApi.TryGetWeekEventsAsync(StartDate, token)
				.RunCatchingFailure()
				.ContinueWith(
					eventsTask =>
					{
						dispatcher.Invoke(
							() =>
							{
								if (eventsTask.Result is not null)
									UpdateFromEventsList(eventsTask.Result);
								else // TODO
									Debug.Fail($"Unable to get events in {nameof(StartRefreshEventsForWeekFromCache)}");
								afterRefresh?.Invoke();
							}
							);
					},
					token
					);
		}

		private void UpdateFromEventsList(IList<IEventDataModel>? events)
		{
			App.AssertUIThread();
			if (events is null)
				return;

			foreach (var dateViewModel in DateViewModels)
			{
				dateViewModel.UpdateFromEventsList(events);
			}
		}

		public void Dispose()
		{
			foreach (var dateViewModel in DateViewModels)
				dateViewModel.Dispose();
		}
	}
}