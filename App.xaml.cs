using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Twinder.View;

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
			bool? returned = true;

			// No login data found, so shows login screen
			if (string.IsNullOrEmpty(Twinder.Properties.Settings.Default.fb_id))
			{
				var loginWindow = new FbLoginView();
				returned = loginWindow.ShowDialog();
				loginWindow.Close();
			}

			// Creates main window if the dialog returned
			if ((bool) returned)
			{
				var mainWindow = new MainWindow();
				//Current.MainWindow = MainWindow;
				mainWindow.Show();
				
			}
		}
	}
}
