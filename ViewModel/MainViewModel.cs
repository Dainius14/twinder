using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.Models;
using Twinder.Models.Updates;
using Twinder.View;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Twinder.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private bool _authenticated = false;

		private UpdatesModel _updates;
		public UpdatesModel Updates
		{
			get { return _updates; }
			set { Set(ref _updates, value); }
		}

		private ObservableCollection<MatchModel> _matchList;
		public ObservableCollection<MatchModel> MatchList
		{
			get { return _matchList; }
			set { Set(ref _matchList, value); }
		}

		private UserModel _user;
		public UserModel User
		{
			get { return _user; }
			set { Set(ref _user, value); }
		}


		public RelayCommand AuthenticateCommand { get; private set; }
		public RelayCommand GetMatchesCommand { get; private set; }
		public RelayCommand UpdateCommand { get; private set; }
		public RelayCommand<MatchModel> OpenChatCommand { get; private set; }
		public RelayCommand<MatchModel> OpenMatchProfileCommand { get; private set; }
		public RelayCommand<MatchModel> UnmatchCommand { get; private set; }
		public RelayCommand GetRecsCommand { get; private set; }
		public RelayCommand SetLocationCommand { get; private set; }
		public RelayCommand ExitCommand { get; private set; }
		public RelayCommand LoginCommand { get; private set; }
		public RelayCommand AboutCommand { get; private set; }


		public MainViewModel()
		{
			GetMatchesCommand = new RelayCommand(GetMatchesComm, CanGetMatches);
			UpdateCommand = new RelayCommand(Update);
			OpenChatCommand = new RelayCommand<MatchModel>((param) => OpenChat(param));
			OpenMatchProfileCommand = new RelayCommand<MatchModel>(param => OpenMatchProfile(param));
			UnmatchCommand = new RelayCommand<MatchModel>(param => Unmatch(param));
			GetRecsCommand = new RelayCommand(GetRecs);
			SetLocationCommand = new RelayCommand(SetLocation);
			ExitCommand = new RelayCommand(Exit);

			LoginCommand = new RelayCommand(Login);
			AboutCommand = new RelayCommand(About);
			
		}
		#region Unmatch command
		private void Unmatch(MatchModel match)
		{
			MessageBox.Show("To be implemented", "Soon");
		}
		#endregion

		public async Task<bool> Authenticate()
		{
			if (await TinderHelper.Authenticate())
			{
				//MessageBox.Show(Application.Current.MainWindow, "Logged in successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				//GetMatches();
				return true;
			}
			else
			{
				//MessageBox.Show(Application.Current.MainWindow, "Authentication error.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		private async void Update()
		{
			var newUpdates = await TinderHelper.GetUpdates(Properties.Settings.Default.last_update);
			if (newUpdates.Matches.Count != 0)
			{
				foreach (var newMatch in newUpdates.Matches)
				{
					var matchToUpdate = MatchList.Where(item => item.Id == newMatch.Id).FirstOrDefault();
					// There's an update to an existing match
					if (matchToUpdate != null)
					{
						// Adds new messages the to list
						foreach (var newMessage in newMatch.Messages)
						{
							if (!matchToUpdate.Messages.Contains(newMessage))
								matchToUpdate.Messages.Add(newMessage);
						}
					}
					// There's a new match
					else
					{
						MatchList.Insert(0, newMatch);
					}
				}
			}
		}

		public async Task<bool> GetMatches()
		{
			if ((Updates = await TinderHelper.GetUpdates()) != null)
			{
				MatchList = Updates.Matches;
				MatchListSetup();
				Properties.Settings.Default["last_update"] = DateTime.UtcNow;
				return true;
			}
			return false;
		}

		private void MatchListSetup()
		{
			MatchList.Reverse();
			var filteredMatchList = MatchList.Where(item => item.Person != null)
				.OrderByDescending(item => item.LastActivityDate);
			MatchList = new ObservableCollection<MatchModel>(filteredMatchList);
		}

		#region Ping command
		private void SetLocation()
		{
			Messenger.Default.Send("ayy", MessageType.ShowSetLocationWindow);
		}
		#endregion
		

		#region Get recommendations command
		public void GetRecs()
		{
			Messenger.Default.Send("ayy", MessageType.ShowRecommendations);
		}
		#endregion

		#region Get matches command
		private async void GetMatchesComm()
		{
			await GetMatches();
		}


		private bool CanGetMatches()
		{
			return !_authenticated;
		}
		#endregion

		#region Open chat command
		private void OpenChat(MatchModel match)
		{
			Messenger.Default.Send(match, MessageType.NewChatWindow);
		}
		#endregion

		#region Open match profile
		private void OpenMatchProfile(MatchModel match)
		{
			Messenger.Default.Send(match, MessageType.ShowMatchProfile);
		}
		#endregion

		#region Login command
		private void Login()
		{
			//Messenger.Default.Send(MessageType.ShowLoginDialog);
			var loggedIn = new NotificationMessageAction<bool>(this, "ShowLoginDialog", async r =>
			{
				if (r)
					await Authenticate();
			});
			Messenger.Default.Send(loggedIn);

		}
		#endregion

		#region Exit command
		/// <summary>
		/// Exits application
		/// </summary>
		private void Exit()
		{
			Application.Current.Shutdown();
		}
		#endregion

		#region About command
		private void About()
		{
			string appName = Properties.Resources.app_title;
			string version = "Version " + Assembly.GetEntryAssembly().GetName().Version.ToString();
			MessageBox.Show(version, appName);

		}
		#endregion
	}
}