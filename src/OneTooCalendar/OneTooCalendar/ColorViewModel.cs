using System.Windows.Media;

namespace OneTooCalendar
{
	public class ColorViewModel : ViewModelBase
	{
		public ColorViewModel(Color color, string name, int? customEventColorId)
		{
			Color = color;
			Name = name;
			CustomEventColorId = customEventColorId;
		}
		public SolidColorBrush Brush => new SolidColorBrush(Color);
		private Color Color { get; }
		public string Name { get; }
		public int? CustomEventColorId { get; }
	}
}