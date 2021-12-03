using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	public interface ICalendarsApi
	{
		Task<ICalendarDataModel[]?> TryGetCalendarsAsync(CancellationToken token);
	}
}