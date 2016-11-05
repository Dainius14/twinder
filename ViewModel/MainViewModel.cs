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

		private string _latitude;
		public string Latitude
		{
			get { return _latitude; }
			set { Set(ref _latitude, value); }
		}

		private string _longtitude;
		public string Longtitude
		{
			get { return _longtitude; }
			set { Set(ref _longtitude, value); }
		}

		public RelayCommand AuthenticateCommand { get; private set; }
		public RelayCommand GetMatchesCommand { get; private set; }
		public RelayCommand UpdateCommand { get; private set; }
		public RelayCommand<MatchModel> OpenChatCommand { get; private set; }
		public RelayCommand GetRecsCommand { get; private set; }
		public RelayCommand PingCommand { get; private set; }
		public RelayCommand<MatchModel> OpenMatchProfileCommand { get; private set; }
		public RelayCommand ExitCommand { get; private set; }
		public RelayCommand LoginCommand { get; private set; }
		public RelayCommand AboutCommand { get; private set; }


		public MainViewModel()
		{
			Longtitude = Properties.Settings.Default.longtitude;
			Latitude = Properties.Settings.Default.latitude;

			GetMatchesCommand = new RelayCommand(GetMatches, CanGetMatches);
			UpdateCommand = new RelayCommand(Update);
			OpenChatCommand = new RelayCommand<MatchModel>((param) => OpenChat(param));
			GetRecsCommand = new RelayCommand(GetRecs);
			PingCommand = new RelayCommand(Ping, CanPing);
			OpenMatchProfileCommand = new RelayCommand<MatchModel>(param => OpenMatchProfile(param));
			ExitCommand = new RelayCommand(Exit);

			LoginCommand = new RelayCommand(Login);
			AboutCommand = new RelayCommand(About);

			Authenticate();
			GetMatches();
		}

		private void Authenticate()
		{
			if (TinderHelper.Authenticate())
			{
				MessageBox.Show(Application.Current.MainWindow, "Logged in successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show(Application.Current.MainWindow, "Authentication error.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void Update()
		{
			var newUpdates = TinderHelper.GetUpdates(Properties.Settings.Default.last_update);
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



		private void MatchListSetup()
		{
			MatchList.Reverse();
			var filteredMatchList = MatchList.Where(item => item.Person != null)
				.OrderByDescending(item => item.LastActivityDate);
			MatchList = new ObservableCollection<MatchModel>(filteredMatchList);
		}

		#region Ping command
		private void Ping()
		{
			Properties.Settings.Default["latitude"] = Latitude;
			Properties.Settings.Default["longtitude"] = Longtitude;
			Properties.Settings.Default.Save();
			TinderHelper.PingLocation(Latitude.Replace(',', '.'), Longtitude.Replace(',', '.'));
		}

		private bool CanPing()
		{
			return (!string.IsNullOrWhiteSpace(Latitude) && !string.IsNullOrWhiteSpace(Longtitude));
		}
		#endregion

		#region Get recommendations command
		private void GetRecs()
		{
			var recsResults = TinderHelper.GetRecommendations();
			if (recsResults.Recommendations != null)
			{
				Messenger.Default.Send(recsResults, MessageType.ShowRecommendations);
			}
			else
			{
				MessageBox.Show(recsResults.Message);
			}
		}
		#endregion

		#region Get matches command
		private void GetMatches()
		{
			if ((Updates = TinderHelper.GetUpdates()) != null)
			{
				MatchList = Updates.Matches;
				MatchListSetup();
				Properties.Settings.Default["last_update"] = DateTime.UtcNow;
			}
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
			var loggedIn = new NotificationMessageAction<bool>(this, "ShowLoginDialog", r =>
			{
				if (r)
					Authenticate();
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
			MessageBox.Show("What do you exactly want to see here?");

		}
		#endregion
	}
}