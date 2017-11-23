using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WpfTestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Task.Run(() => throw new Exception("TEST"));
		}

		private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.GetBaseException().ToString());
			e.SetObserved();
		}

		private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.ToString());
			e.Handled = true;
		}
	}
}
