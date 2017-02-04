using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.View;
using System.Xml.Linq;

namespace Twinder.ViewModel
{
	public class AccountSwitchViewModel : ViewModelBase
	{ 
		private string _fbId;
		public string FbId
		{
			get { return _fbId; }
			set { Set(ref _fbId, value); }
		}

		private string _fbToken;
		public string FbToken
		{
			get { return _fbToken; }
			set { Set(ref _fbToken, value); }
		}

		private bool _setAsDefault;
		public bool SetAsDefault
		{
			get { return _setAsDefault; }
			set { Set(ref _setAsDefault, value); }
		}

		public bool IsOkEnabled { get { return Selected != null; } }
		public string UserName { get; private set; }


		public RelayCommand<Window> AddNewAccountCommand { get; private set; }

		public RelayCommand<Window> OkCommand { get; private set; }

		private ObservableCollection<AccountModel> _accountList;
		public ObservableCollection<AccountModel> AccountList
		{
			get { return _accountList; }
			set { Set(ref _accountList, value); }
		}

		private AccountModel _selected;
		public AccountModel Selected
		{
			get { return _selected; }
			set { Set(ref _selected, value); }
		}

		public AccountSwitchViewModel()
		{
			AddNewAccountCommand = new RelayCommand<Window>(param => AddNewAccount(param));
			OkCommand = new RelayCommand<Window>(param => Ok(param));


			// Populates ListView with accounts that are in the folder
			AccountList = new ObservableCollection<AccountModel>();
			foreach (var accDir in Directory.GetDirectories(Properties.Settings.Default.AppDataFolder))
			{
				var file = accDir + "\\" + SerializationHelper.DIR_USER + SerializationHelper.USER_FILE;
				var content = File.ReadAllText(file);
				var user = JsonConvert.DeserializeObject<UserModel>(content);
				var matchCount = Directory.GetDirectories(accDir + "\\" + SerializationHelper.DIR_MATCHES).Length;
				var account = new AccountModel()
				{
					AccoutName = user.ToString(),
					Name = user.Name,
					MatchCount = matchCount,
					Photo = user.Photos[0].FileName
				};
				AccountList.Add(account);

			}

			if (AccountList.Count > 0)
				Selected = AccountList[0];
		}

		private void Ok(Window param)
		{
			// Reads XML with login data
			var configPath = Properties.Settings.Default.AppDataFolder + Selected + "\\config.xml";

			var doc = XDocument.Load(configPath);
			FbId = doc.Element("LoginData").Element("FbId").Value;
			FbToken = doc.Element("LoginData").Element("FbToken").Value;

			UserName = Selected.ToString();

			if (SetAsDefault)
				Properties.Settings.Default.DefaultUser = UserName;


			param.DialogResult = true;
		}

		private void AddNewAccount(Window param)
		{
			var loginWindow = new FbLoginView();

			// Waits for user to press OK
			if (loginWindow.ShowDialog() == true)
			{
				var loginVM = loginWindow.DataContext as FbLoginViewModel;
				FbId = loginVM.FbId;
				FbToken = loginVM.FbToken;
				param.DialogResult = true;
			}
		}
	}
}
