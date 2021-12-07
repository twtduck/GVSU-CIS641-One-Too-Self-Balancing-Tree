using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OneTooCalendar
{
	public partial class BacklogEventView : UserControl
	{
		public BacklogEventView()
		{
			InitializeComponent();
			MouseLeftButtonDown += OnMouseLeftButtonDown;
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DragDrop.DoDragDrop(this, (BacklogEventViewModel)DataContext, DragDropEffects.Move);
		}
	}
}