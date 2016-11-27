using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Twinder.Helpers;
using Twinder.Properties;
using Twinder.View;

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
		public RelayCommand<Window> CancelCommand { get; private set; }

		public FbLoginViewModel()
		{
			FbId = Settings.Default.fb_id;
			FbToken = Settings.Default.fb_token;


			GetFbIdCommand = new RelayCommand(GetFbId);
			GetFbTokenCommand = new RelayCommand(GetFbToken);
			SetUserCommand = new RelayCommand<Window>(SetUser, CanSetUser);
			CancelCommand = new RelayCommand<Window>(Cancel);
		}

		#region Get Fb ID command
		/// <summary>
		/// Opens browser for user to get his Fb ID;
		/// </summary>
		private void GetFbId()
		{
			System.Diagnostics.Process.Start(ID_URL);
		}
		#endregion

		#region Get Fb Token command
		/// <summary>
		/// Opens browser for user to get his Fb token from the guide
		/// </summary>
		private void GetFbToken()
		{
			System.Diagnostics.Process.Start(GUIDE_URL);
		}
		#endregion

		#region Set User command
		/// <summary>
		/// Saves ID and token
		/// </summary>
		private void SetUser(Window window)
		{
			Settings.Default["fb_id"] = FbId;
			Settings.Default["fb_token"] = FbToken;
			Settings.Default.Save();

			window.DialogResult = true;
		}

		/// <summary>
		/// Checks if ID and token are not empty or the same before saving
		/// </summary>
		/// <returns></returns>
		private bool CanSetUser(Window window)
		{
			if (!string.IsNullOrWhiteSpace(FbId) && !string.IsNullOrWhiteSpace(FbToken))
				if (FbId != Settings.Default.fb_id || FbToken != Settings.Default.fb_token)
					return true;
			return false;
		}
		#endregion

		#region Cancel command
		/// <summary>
		/// Closes application
		/// </summary>
		private void Cancel(Window window)
		{
			window.DialogResult = false;
		}
		#endregion


	}
}
