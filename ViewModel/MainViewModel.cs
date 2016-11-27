using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Twinder.Helpers;
using Twinder.Models;
using Twinder.Models.Updates;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Twinder.Model;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Twinder.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		public delegate void ConnectionStatusHandler(object sender, ConnectionStatusEventArgs e);
		public event ConnectionStatusHandler ConnectionStatusChanged;

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

		private ObservableCollection<RecModel> _recList;
		public ObservableCollection<RecModel> RecList
		{
			get { return _recList; }
			set { Set(ref _recList, value); }
		}

		private UserModel _user;
		public UserModel User
		{
			get { return _user; }
			set { Set(ref _user, value); }
		}

		// Holds reference to my view to subsribe to events
		public MainWindow MyView { get; set; }

		public AuthStatus AuthStatus { get; set; }

		public RelayCommand<MatchModel> OpenChatCommand { get; private set; }
		public RelayCommand<MatchModel> OpenMatchProfileCommand { get; private set; }
		public RelayCommand<MatchModel> UnmatchCommand { get; private set; }

		public RelayCommand OpenRecsCommand { get; private set; }
		public RelayCommand OpenUserProfileCommand { get; private set; }
		public RelayCommand SetLocationCommand { get; private set; }
		public RelayCommand ExitCommand { get; private set; }
		public RelayCommand LoginCommand { get; private set; }
		public RelayCommand AboutCommand { get; private set; }


		private DateTime _lastActivity;

		public MainViewModel()
		{
			OpenChatCommand = new RelayCommand<MatchModel>((param) => OpenChat(param));
			OpenMatchProfileCommand = new RelayCommand<MatchModel>(param => OpenMatchProfile(param));
			UnmatchCommand = new RelayCommand<MatchModel>(param => Unmatch(param));

			OpenRecsCommand = new RelayCommand(OpenRecs, IsConnected);
			OpenUserProfileCommand = new RelayCommand(OpenUserProfile, IsConnected);
			SetLocationCommand = new RelayCommand(SetLocation, IsConnected);

			ExitCommand = new RelayCommand(Exit);

			LoginCommand = new RelayCommand(Login);
			AboutCommand = new RelayCommand(About);

			_lastActivity = DateTime.UtcNow;

			Messenger.Default.Register<string>(this, MessengerToken.ForceUpdate, AddNewMatch);
		}

		private bool IsConnected()
		{
			return AuthStatus == AuthStatus.Okay;
		}

		private void AddNewMatch(string message)
		{
			UpdateMatches(this, null);
		}
		public async void StartConnection(object sender, EventArgs e)
		{
			ConnectionStatusEventArgs args = new ConnectionStatusEventArgs();

			// Starts authentication
			if (await Authenticate() == AuthStatus.Okay)
			{
				args.AuthStatus = AuthStatus.Okay;
				args.MatchesStatus = MatchesStatus.Getting;
				args.RecsStatus = RecsStatus.Getting;
				ConnectionStatusChanged.Invoke(this, args);

				// Gets matches
				args.MatchesStatus = await GetMatches();
				ConnectionStatusChanged.Invoke(this, args);

				// Gets recs
				args.RecsStatus = await GetRecs();
				ConnectionStatusChanged.Invoke(this, args);

				
				if (args.MatchesStatus == MatchesStatus.Okay)
					StartUpdatingMatches();

				if (args.RecsStatus == RecsStatus.Okay || args.RecsStatus == RecsStatus.Exhausted)
					StartUpdatingRecs();
			}
			else
				args.AuthStatus = AuthStatus.Error;

		}

		private void StartUpdatingMatches()
		{
			DispatcherTimer timerUpdates = new DispatcherTimer();
			timerUpdates.Tick += UpdateMatches;
			timerUpdates.Interval = new TimeSpan(0, 0, 10);
			timerUpdates.Start();
		}


		private void TimerUpdates_Tick(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void StartUpdatingRecs()
		{
			//throw new NotImplementedException();
		}

		#region Unmatch command
		private void Unmatch(MatchModel match)
		{
			MessageBox.Show("To be implemented", "Soon");
		}
		#endregion

		public async Task<AuthStatus> Authenticate()
		{
			if (await TinderHelper.Authenticate())
			{
				User = TinderHelper.User;
				// Sets current location of user
				Properties.Settings.Default["latitude"] = User.Pos.Latitude.Replace('.', ',');
				Properties.Settings.Default["longtitude"] = User.Pos.Longtitude.Replace('.', ',');
				Properties.Settings.Default.Save();
				AuthStatus = AuthStatus.Okay;
				return AuthStatus.Okay;
			}
			AuthStatus = AuthStatus.Error;
			return AuthStatus.Error;
		}


		/// <summary>
		/// Tries connecting to Tinder servers and getting updates
		/// </summary>
		/// <returns></returns>
		public async Task<MatchesStatus> GetMatches()
		{
			if ((Updates = await TinderHelper.GetUpdates()) != null)
			{
				_lastActivity = Updates.LastActivityDate;
				MatchList = Updates.Matches;

				MatchListSetup();
				Properties.Settings.Default["last_update"] = DateTime.UtcNow;
				return MatchesStatus.Okay;
			}
			return MatchesStatus.Error;
		}
		/// <summary>
		/// Tries connecting to Tinder servers and getting recommendations
		/// </summary>
		/// <returns></returns>
		public async Task<RecsStatus> GetRecs()
		{
			var recs = await TinderHelper.GetRecommendations();
			if (recs != null)
			{
				if (recs.Recommendations != null)
				{
					RecList = new ObservableCollection<RecModel>(recs.Recommendations);
					return RecsStatus.Okay;
				}
				return RecsStatus.Exhausted;
			}
			return RecsStatus.Error;
		}

		private async void UpdateMatches(object sender, EventArgs e)
		{
			var newUpdates = await TinderHelper.GetUpdates(_lastActivity);

			if (newUpdates.Matches.Count != 0)
			{
				_lastActivity = newUpdates.LastActivityDate;
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
						matchToUpdate.LastActivityDate = newMatch.LastActivityDate;
						Messenger.Default.Send("", MessengerToken.SortMatchList);
					}
					// There's a new match
					else
					{
						MatchList.Insert(0, newMatch);
					}
				}
			}
		}

		/// <summary>
		/// Removes matches with null value (don't know why they are there) and 
		/// sorts by LastActivityDate in descending order
		/// </summary>
		private void MatchListSetup()
		{
			// Adds event handlers for each message list
			foreach (var item in MatchList)
			{
				item.Messages.CollectionChanged += Messages_CollectionChanged;
			}

			var filteredMatchList = MatchList.Where(item => item.Person != null)
				.OrderByDescending(item => item.LastActivityDate).ToList();
			MatchList = new ObservableCollection<MatchModel>(filteredMatchList);
		}

		/// <summary>
		/// If there are new messages added to collection, sends message to View to update bindings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Messenger.Default.Send("", MessengerToken.RefreshMatchList);

		}

		private void OpenUserProfile()
		{
			Messenger.Default.Send(User, MessengerToken.OpenMyProfile);
		}
		
		private void SetLocation()
		{
			Messenger.Default.Send("ayy", MessengerToken.ShowSetLocationWindow);
		}
		

		public void OpenRecs()
		{
			Messenger.Default.Send(RecList, MessengerToken.OpenRecommendations);
		}

		private void OpenChat(MatchModel match)
		{
			Messenger.Default.Send(match, MessengerToken.NewChatWindow);
		}

		private void OpenMatchProfile(MatchModel match)
		{
			Messenger.Default.Send(match, MessengerToken.ShowMatchProfile);
		}

		private void Login()
		{
			Messenger.Default.Send("", MessengerToken.ShowLoginDialog);

		}

		private void About()
		{
			string appName = Properties.Resources.app_title;
			string version = "Version " + Assembly.GetEntryAssembly().GetName().Version.ToString();
			MessageBox.Show(version, appName);

		}

		/// <summary>
		/// Exits application
		/// </summary>
		private void Exit()
		{
			Application.Current.Shutdown();
		}
	}

	public class ConnectionStatusEventArgs : EventArgs
	{
		public AuthStatus AuthStatus { get; set; }
		public MatchesStatus MatchesStatus { get; set; }
		public RecsStatus RecsStatus{ get; set; }

		public ConnectionStatusEventArgs()
		{
			AuthStatus = AuthStatus.Connecting;
			MatchesStatus = MatchesStatus.Waiting;
			RecsStatus = RecsStatus.Waiting;
		}
	}

	public enum AuthStatus
	{
		Connecting,
		Okay,
		Error
	}

	public enum MatchesStatus
	{
		Waiting,
		Getting,
		Okay,
		Error
	}

	public enum RecsStatus
	{
		Waiting,
		Getting,
		Okay,
		Exhausted,
		Error
	}
}