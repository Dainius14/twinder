using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Twinder.View;
using Twinder.ViewModel;
using Twinder.Properties;

namespace Twinder
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		static App()
		{
			DispatcherHelper.Initialize();
		}

		/// <summary>
		/// Evaluates wether to launch Fb login window or not
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// If version is upgraded, copies user settings
			if (Settings.Default.UpgradeRequired)
			{
				Settings.Default.Upgrade();
				Settings.Default["UpgradeRequired"] = false;
				Settings.Default.Save();
			}

			bool? returned = true;

			// No login data found, so shows login screen
			if (string.IsNullOrEmpty(Settings.Default.fb_id))
			{
				var loginWindow = new FbLoginView();
				returned = loginWindow.ShowDialog();
				loginWindow.Close();
			}

			// Creates main window if the dialog returned
			if ((bool) returned)
			{
				var mainWindow = new MainWindow();
				var mainViewModel = mainWindow.DataContext as MainViewModel;
				mainWindow.ContentRendered += mainViewModel.StartConnection;
				mainWindow.Show();
			}
			else
			{
				Shutdown();
			}
		}

		/// <summary>
		/// Shows a message box if an unhandled exception is thrown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			var errorDialog = new ErrorDialogView(e.Exception);
			errorDialog.ShowDialog();
		}
	}
}
