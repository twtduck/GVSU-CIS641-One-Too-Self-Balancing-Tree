using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace OneTooCalendar
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		public static void AssertUIThread() =>
			Debug.Assert(Dispatcher.CurrentDispatcher.Thread == Thread.CurrentThread, "This method must be called on the UI thread.");
	}
}
