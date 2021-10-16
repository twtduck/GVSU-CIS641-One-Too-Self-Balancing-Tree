using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OneTooCalendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            InitializeApplicationAsync().RunCatchingFailure();
        }

        async Task InitializeApplicationAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            MainWindowViewModel.CurrentView = new CalendarViewModel();
        }

        public MainWindowViewModel MainWindowViewModel { get; } = new MainWindowViewModel();
    }

    public class CalendarViewModel : ViewModelBase
    {
        private CalendarWeekViewModel _calendarWeekViewModel;
        private CalendarControlsBarViewModel _calendarControlsBarViewModel;
        private BacklogViewModel _backlogViewModel;

        public CalendarViewModel()
        {
            _calendarWeekViewModel = new CalendarWeekViewModel();
            _calendarControlsBarViewModel = new CalendarControlsBarViewModel();
            _backlogViewModel = new BacklogViewModel();
        }

        public CalendarWeekViewModel CalendarWeekViewModel
        {
            get => _calendarWeekViewModel;
            set
            {
                _calendarWeekViewModel = value;
                OnPropertyChanged();
            }
        }
        
        public CalendarControlsBarViewModel CalendarControlsBarViewModel
        {
            get => _calendarControlsBarViewModel;
            set
            {
                _calendarControlsBarViewModel = value;
                OnPropertyChanged();
            }
        }
        
        public BacklogViewModel BacklogViewModel
        {
            get => _backlogViewModel;
            set
            {
                _backlogViewModel = value;
                OnPropertyChanged();
            }
        }
    }

    public class BacklogViewModel : ViewModelBase { }

    public class CalendarControlsBarViewModel : ViewModelBase { }

    public class CalendarWeekViewModel : ViewModelBase { }

    public static class TaskExtensions
    {
        public static T RunCatchingFailure<T>(this T task) where T : Task
        {
            if (task.IsCompleted)
            {
                if (task.IsFaulted)
                {
                    Debug.Fail(task.Exception?.Message ?? "No exception");
                }
                return task;
            }

            task.ContinueWith(
                _ => Debug.Fail(task.Exception?.Message ?? "No exception"),
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
                );

            return task;
        }
    }
}
