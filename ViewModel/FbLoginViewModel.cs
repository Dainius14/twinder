using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows;

namespace Twinder.ViewModel
{
	public class FbLoginViewModel : ViewModelBase
	{
		private const string ID_URL = @"http://findmyfbid.com/";
		private const string GUIDE_URL = @"https://gist.github.com/rtt/10403467#gistcomment-1846343";

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
		

		public RelayCommand GetFbIdCommand { get; private set; }
		public RelayCommand GetFbTokenCommand { get; private set; }
		public RelayCommand<Window> SetUserCommand { get; private set; }

		public FbLoginViewModel()
		{
			GetFbIdCommand = new RelayCommand(() => System.Diagnostics.Process.Start(ID_URL));
			GetFbTokenCommand = new RelayCommand(() => System.Diagnostics.Process.Start(GUIDE_URL));
			SetUserCommand = new RelayCommand<Window>(
				param => param.DialogResult = true, 
				param => !string.IsNullOrWhiteSpace(FbId) && !string.IsNullOrWhiteSpace(FbToken));
		}
		
	}
}
