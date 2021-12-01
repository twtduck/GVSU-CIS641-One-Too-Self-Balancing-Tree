using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	public class CalendarWeekViewModel : ViewModelBase, IDisposable
	{
		private readonly GoogleCalendarService _googleCalendarService;
		public const int DaysInAWeek = 7;

		public CalendarWeekViewModel(DateTime startDate, GoogleCalendarService googleCalendarService)
		{
			_googleCalendarService = googleCalendarService;
			for (var i = 0; i < DaysInAWeek; i++)
			{
				DateViewModels.Add(new DateViewModel(startDate + TimeSpan.FromDays(i), new EventCommandFactory(_googleCalendarService))
				{
					BorderOpacity = i == 0 ? 0 : 1
				});
			}

			StartDate = startDate;
		}

		public DateTime StartDate { get; }

		public List<DateViewModel> DateViewModels { get; } = new();

		public List<HourLabelViewModel> TimeLabels =>
			Enumerable.Range(0, 24)
				.Select(hourNumber => new HourLabelViewModel(hourNumber))
				.ToList();

		public async Task<bool> TryRefreshEventsAsync(CancellationToken token)
		{
			var events = await _googleCalendarService.GetEventsForDateRangeAsync(StartDate, StartDate.AddDays(DaysInAWeek), token);
			if (events is null)
				return false;
			foreach (var dateViewModel in DateViewModels)
			{
				dateViewModel.UpdateFromEventsList(events);
			}
			return true;

		}

		public void Dispose()
		{
			foreach (var dateViewModel in DateViewModels)
				dateViewModel.Dispose();
		}
	}
}