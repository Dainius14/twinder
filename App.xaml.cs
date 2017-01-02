using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Twinder.View;
using Twinder.ViewModel;
using Twinder.Properties;
using System;
using System.IO;
using Twinder.Helpers;

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
		/// Start logic right here
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			// If version is upgraded, copies user settings from previous version
			if (Settings.Default.UpgradeRequired)
			{
				Settings.Default.Upgrade();
				Settings.Default["UpgradeRequired"] = false;
				Settings.Default.Save();
			}
			

			// First start configuration
			if (Settings.Default.FirstStart)
			{
				// App data will be in \Local\Tinder\ folder
				Settings.Default["AppDataFolder"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
					+ "\\Twinder\\";

				// checks if the folder exists
				if (!Directory.Exists(Settings.Default.AppDataFolder))
					SerializationHelper.CreateFolderStructure(Settings.Default.AppDataFolder);

			}
			bool? returned = true;

			// No login data found, so shows login screen
			if (string.IsNullOrEmpty(Settings.Default.FbId))
			{
				var loginWindow = new FbLoginView();
				returned = loginWindow.ShowDialog();
				loginWindow.Close();
			}

			// Creates main window if the dialog returned
			if ((bool) returned)
			{
				var mainWindow = new MainWindow();
				var MainViewModel = mainWindow.DataContext as MainViewModel;
				// Once loaded, starts setting up all the data
				mainWindow.Loaded += MainViewModel.StartInitialize;
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
