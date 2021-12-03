using System.Windows.Media;

namespace OneTooCalendar
{
	public class ColorViewModel : ViewModelBase
	{
		private Color _color;

		public ColorViewModel(Color color, string name, int customEventColorId)
		{
			_color = color;
			Name = name;
			CustomEventColorId = customEventColorId;
		}
		public SolidColorBrush Brush => new SolidColorBrush(Color);

		public Color Color
		{
			get => _color;
			set
			{
				_color = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Brush));
			}
		}

		public string Name { get; }
		public int CustomEventColorId { get; }
	}
}