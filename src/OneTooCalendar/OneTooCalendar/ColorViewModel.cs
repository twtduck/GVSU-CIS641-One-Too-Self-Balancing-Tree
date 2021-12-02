using System.Windows.Media;

namespace OneTooCalendar
{
	public class ColorViewModel : ViewModelBase
	{
		public ColorViewModel(Color color, string name)
		{
			Color = color;
			Name = name;
		}
		public SolidColorBrush Brush => new SolidColorBrush(Color);
		public Color Color { get; }
		public string Name { get; }
	}
}