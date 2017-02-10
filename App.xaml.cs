using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using Twinder.View;
using Twinder.ViewModel;
using Twinder.Properties;
using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace Twinder
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public const string ARG_SHOW_ACCOUNT_SWITCHER = "account_switcher";

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
			// First start configuration
			if (Settings.Default.FirstStart)
			{
				// App data will be in %appdata%\Local\Twinder\ folder
				Settings.Default["AppDataFolder"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
					+ "\\Twinder\\";
				
				if (!Directory.Exists(Settings.Default.AppDataFolder))
					Directory.CreateDirectory(Settings.Default.AppDataFolder);

			}
			
			string fbId = string.Empty;
			string fbToken = string.Empty;
			string userName = string.Empty;

			// Logs in default user, otherwise shows account switcher
			if (!string.IsNullOrEmpty(Settings.Default.DefaultUser) && !e.Args.Contains(ARG_SHOW_ACCOUNT_SWITCHER))
			{
				userName = Settings.Default.DefaultUser;

				// Loads XML
				var doc = XDocument.Load(Settings.Default.AppDataFolder + userName + "\\config.xml");

				fbId = doc.Element("LoginData").Element("FbId").Value;
				fbToken = doc.Element("LoginData").Element("FbToken").Value;
			}
			// No default user or arg given
			else
			{
				var accountSwitchWindow = new AccountSwitchView();
				var accSwitchVM = accountSwitchWindow.DataContext as AccountSwitchViewModel;
				if (accountSwitchWindow.ShowDialog() == true)
				{
					userName = accSwitchVM.UserName;
					fbId = accSwitchVM.FbId;
					fbToken = accSwitchVM.FbToken;
				}
				else
					Shutdown();

			}
			
			var mainWindow = new MainWindow();
			var mainVM = mainWindow.DataContext as MainViewModel;
			mainVM.UserName = userName;
			mainVM.FbId = fbId;
			mainVM.FbToken = fbToken;
			mainWindow.Show();
			
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
