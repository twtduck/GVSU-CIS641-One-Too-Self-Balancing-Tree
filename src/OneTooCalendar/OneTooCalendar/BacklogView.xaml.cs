using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OneTooCalendar
{
	public partial class BacklogView : UserControl
	{
		public BacklogView()
		{
			InitializeComponent();
			InitDragDrop(this);
			InitDragDrop(emptyBacklogLabel);
			InitDragDrop(grid);
			InitDragDrop(eventsList);
			InitDragDrop(scrollViewer);
		}

		private void InitDragDrop(UIElement backlogView)
		{
			backlogView.DragOver += (sender, args) =>
			{
				if (args.Handled)
					return;

				(DataContext as BacklogViewModel)!.OnDragOver(args);
				args.Handled = true;
			};
			backlogView.DragLeave += (sender, args) =>
			{
				if (args.Handled)
					return;

				(DataContext as BacklogViewModel)!.OnDragLeave(args);
				args.Handled = true;
			};
			backlogView.Drop += (sender, args) =>
			{
				if (args.Handled)
					return;

				(DataContext as BacklogViewModel)!.OnDrop(args);
				args.Handled = true;
			};
		}
	}
}