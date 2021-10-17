using System;
using System.Windows.Media;

namespace OneTooCalendar
{
    public interface IEventViewModel
    {
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        string Title { get; }
        bool AllDayEvent { get; }
        Color Color { get; }
    }
}