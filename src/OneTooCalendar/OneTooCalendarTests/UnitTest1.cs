using System.Windows;
using FluentAssertions;
using OneTooCalendar;
using Xunit;

namespace OneTooCalendarTests
{
	public class DragDropHelperTests
	{
		// [Theory]
		// [InlineData(0, 0, 0, 0, 0, 0, 0, 0)]
		// [InlineData(0, 0, 0, 0, 0, 11, 0, 0)]
		// public void TestGetNewEventPosition(
		// 	int startRelativeToEventX,
		// 	int startRelativeToEventY,
		// 	int startRelativeToDateX,
		// 	int startRelativeToDateY,
		// 	int endRelativeToDateX,
		// 	int endRelativeToDateY,
		// 	int expectedDaysRight,
		// 	int expectedQuarterHourPeriodsDown
		// 	)
		// {
		// 	var helper = new DragDropHelper(new Point(startRelativeToEventX, startRelativeToEventY), new Point(startRelativeToDateX, startRelativeToDateY));
		// 	var (daysRight, quarterHourPeriodsDown) = helper.GetNewEventPosition(new Point(endRelativeToDateX, endRelativeToDateY));
		// 	daysRight.Should().Be(expectedDaysRight);
		// 	quarterHourPeriodsDown.Should().Be(expectedQuarterHourPeriodsDown);
		// }
	}
}