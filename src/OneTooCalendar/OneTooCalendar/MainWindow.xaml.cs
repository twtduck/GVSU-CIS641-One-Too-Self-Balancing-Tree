using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OneTooCalendar
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			DataContext = this;
			InitializeComponent();
			StartInitializeApplication();
		}

		private void StartInitializeApplication()
		{
			MainWindowViewModel.CurrentView = new SynchronizingCalendarViewModel();
			var initTimeToken = new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token;
			InitializeApplicationAsync(initTimeToken).RunCatchingFailure();
		}

		async Task InitializeApplicationAsync(CancellationToken token)
		{
			var googleCalendarApi = new GoogleCalendarApi();
			var connectSucceed = await googleCalendarApi.InitializeServiceAsync(token) && !token.IsCancellationRequested;
			if (connectSucceed)
			{
				var eventsApi = new EventsCache(googleCalendarApi);
				var calendarsApi = new CalendarsCache(googleCalendarApi);
				var calendar = new CalendarViewModel(eventsApi, calendarsApi);
				calendar.SetMainViewTemporarily = temporaryViewModel =>
				{
					App.AssertUIThread();
					MainWindowViewModel.CurrentView = temporaryViewModel;
					return () =>
					{
						App.AssertUIThread();
						MainWindowViewModel.CurrentView = calendar;
					};
				};
				Closing += (_, _) => calendar.Dispose();
				calendar.CalendarWeekViewModel.StartClearCachesAndUpdateEvents(eventsApi, () => MainWindowViewModel.CurrentView = calendar);
			}
			else
			{
				MainWindowViewModel.CurrentView = new InitializationErrorViewModel(StartInitializeApplication);
			}
		}

		public MainWindowViewModel MainWindowViewModel { get; } = new MainWindowViewModel();
	}

	public static class TaskExtensions
	{
		public static T RunCatchingFailure<T>(this T task) where T : Task
		{
			if (task.IsCompleted)
			{
				if (task.IsFaulted)
				{
					HandleFailedTask();
				}
				return task;
			}

			task.ContinueWith(
				_ => HandleFailedTask(),
				TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
				);

			return task;

			void HandleFailedTask()
			{
				if (Debugger.IsAttached)
					Debugger.Break();
				Debug.Fail(task.Exception?.Message ?? "No exception");
			}
		}
	}
}
