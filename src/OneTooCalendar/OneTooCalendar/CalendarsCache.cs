using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	public class CalendarsCache : ICalendarsApi
	{
		private readonly GoogleCalendarApi _googleCalendarApi;

		public CalendarsCache(GoogleCalendarApi googleCalendarApi)
		{
			_googleCalendarApi = googleCalendarApi;
		}

		public Task<ICalendarDataModel[]?> TryGetCalendarsAsync(CancellationToken token)
		{
			return _googleCalendarApi.GetCalendarsAsync(token);
		}
	}
}