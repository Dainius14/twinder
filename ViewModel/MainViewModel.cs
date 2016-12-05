using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Threading;
using Twinder.Model.Authentication;
using System.Threading;
using System.IO;

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

		private AuthModel _auth;
		public AuthModel Auth
		{
			get { return _auth; }
			set { Set(ref _auth, value); }
		}

		// Holds reference to my view to subsribe to events
		public MainWindow MyView { get; set; }

		public AuthStatus AuthStatus { get; private set; }
		public MatchesStatus MatchesStatus { get; private set; }
		public RecsStatus RecsStatus { get; private set; }

		public RelayCommand<MatchModel> OpenChatCommand { get; private set; }
		public RelayCommand<MatchModel> OpenMatchProfileCommand { get; private set; }
		public RelayCommand<MatchModel> UnmatchCommand { get; private set; }

		public RelayCommand OpenRecsCommand { get; private set; }
		public RelayCommand OpenUserProfileCommand { get; private set; }
		public RelayCommand SetLocationCommand { get; private set; }
		public RelayCommand ExitCommand { get; private set; }
		public RelayCommand LoginCommand { get; private set; }
		public RelayCommand AboutCommand { get; private set; }
		public RelayCommand ForceDownloadMatchesCommand { get; private set; }
		public RelayCommand ForceDownloadRecsCommand { get; private set; }

		/// <summary>
		/// Matches are added here when they receive an update or have new messages
		/// </summary>
		public ObservableCollection<MatchModel> UpdatedMatches { get; private set; }

		public MainViewModel()
		{
			UpdatedMatches = new ObservableCollection<MatchModel>();

			OpenChatCommand = new RelayCommand<MatchModel>((param) => OpenChat(param));
			OpenMatchProfileCommand = new RelayCommand<MatchModel>(param => OpenMatchProfile(param));
			UnmatchCommand = new RelayCommand<MatchModel>(param => Unmatch(param));

			OpenRecsCommand = new RelayCommand(OpenRecs, () =>
			{
				return IsConnected() && RecList != null && RecList.Count != 0;
			});

			OpenUserProfileCommand = new RelayCommand(OpenUserProfile, IsConnected);
			SetLocationCommand = new RelayCommand(SetLocation, IsConnected);

			ExitCommand = new RelayCommand(Exit);

			LoginCommand = new RelayCommand(Login);
			AboutCommand = new RelayCommand(About);
			ForceDownloadMatchesCommand = new RelayCommand(ForceDownloadMatches);
			ForceDownloadRecsCommand = new RelayCommand(ForceDownloadRecs);

			Messenger.Default.Register<string>(this, MessengerToken.ForceUpdate, AddNewMatch);
			Messenger.Default.Register<string>(this, MessengerToken.GetMoreRecs, GetMoreRecs);

			Application.Current.Exit += Current_Exit;
		}
		private async void GetMoreRecs(string obj)
		{
			await GetRecs();
			Messenger.Default.Send(new SerializationPacket(RecList), MessengerToken.ShowSerializationDialog);
		}

		public async Task<bool> FullConnect()
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
				return true;
			}
			else
			{
				args.AuthStatus = AuthStatus.Error;
				ConnectionStatusChanged.Invoke(this, args);
				return false;
			}
		}


		/// <summary>
		/// First thing called. Entry point of some sort. Calls other methods to get Tinder data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public async void StartInitialize(object sender, EventArgs e)
		{
			// Only do full authorization if it's the first time or data is not saved
			if (Properties.Settings.Default.FirstStart || !Properties.Settings.Default.SerializationComplete)
			{
				if (await FullConnect())
				{
					
					int time = MatchList.Count * 3 / 60;
					var result = MessageBox.Show($"Do you want to save all match data? It may take up to {time} minutes.",
						"Saved data not found", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (result == MessageBoxResult.Yes)
						Messenger.Default.Send(new SerializationPacket(MatchList, RecList, User),
							MessengerToken.ShowSerializationDialog);
					
					StartUpdatingMatches();
					StartUpdatingRecs();
				}

			}
			else
			{
				ConnectionStatusEventArgs args = new ConnectionStatusEventArgs();
				if (await Authenticate() == AuthStatus.Okay)
				{
					args.AuthStatus = AuthStatus.Okay;
					args.MatchesStatus = MatchesStatus.Getting;
					args.RecsStatus = RecsStatus.Getting;
					ConnectionStatusChanged.Invoke(this, args);

					// Deserializes matches
					MatchList = SerializationHelper.DeserializeMatchList();
					MatchListSetup();
					args.MatchesStatus = MatchesStatus.Okay;
					ConnectionStatusChanged.Invoke(this, args);

					// Deserializes recs
					RecList = SerializationHelper.DeserializeRecList();
					if (RecList.Count == 0)
					{
						await GetRecs();
						Messenger.Default.Send(new SerializationPacket(RecList),
							MessengerToken.ShowSerializationDialog);

					}

					args.RecsStatus = RecsStatus.Okay;
					ConnectionStatusChanged.Invoke(this, args);

					// Gets updates
					UpdateMatches(this, null);

					// Starts automatic updates
					UpdateMatches(this, null);
					UpdateRecs(this, null);
					StartUpdatingMatches();
					StartUpdatingRecs();

				}

			}
			Messenger.Default.Send("", MessengerToken.SortMatchList);
		}

		/// <summary>
		/// Tries authenticating with Tinder servers and getting User Data
		/// </summary>
		/// <returns></returns>
		public async Task<AuthStatus> Authenticate()
		{
			// Tinder token is empty, so tries to acquire it

			try
			{
				// If no tinder token is present, gets it from 
				Auth = await TinderHelper.GetAuthData(Properties.Settings.Default.fb_id, Properties.Settings.Default.fb_token);
				Properties.Settings.Default["TinderToken"] = Auth.Token;

				User = await TinderHelper.GetFullUserData();

				Properties.Settings.Default["LastUpdate"] = User.LatestUpdateDate;
				
				// Updates current location of user
				Properties.Settings.Default["latitude"] = User.Pos.Latitude.Replace('.', ',');
				Properties.Settings.Default["longtitude"] = User.Pos.Longtitude.Replace('.', ',');
				Properties.Settings.Default.Save();

				AuthStatus = AuthStatus.Okay;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);

				AuthStatus = AuthStatus.Error;
			}
			return AuthStatus;
		}

		/// <summary>
		/// Tries connecting to Tinder servers and getting updates
		/// </summary>
		/// <returns></returns>
		public async Task<MatchesStatus> GetMatches()
		{
			try
			{
				Updates = await TinderHelper.GetUpdates();
				MatchList = Updates.Matches;

				MatchListSetup();

				Properties.Settings.Default["LastUpdate"] = Updates.LastActivityDate;
				Properties.Settings.Default.Save();
				MatchesStatus = MatchesStatus.Okay;
			}
			catch (TinderRequestException e)
			{
				MatchesStatus = MatchesStatus.Error;
			}
			return MatchesStatus;
		}
		
		/// <summary>
		/// Tries connecting to Tinder servers and getting recommendations
		/// </summary>
		/// <returns></returns>
		public async Task<RecsStatus> GetRecs()
		{
			try
			{
				var recs = await TinderHelper.GetRecommendations();
				if (recs.Recommendations != null)
				{
					// If it's the first time getting recs
					if (RecList == null)
						RecList = new ObservableCollection<RecModel>(recs.Recommendations);
					// If it's recs update, just inserts new recs to current list
					else
						foreach (var item in recs.Recommendations)
							RecList.Add(item);

					RecsStatus = RecsStatus.Okay;
				}
				else
					RecsStatus = RecsStatus.Exhausted;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
				RecsStatus = RecsStatus.Error;
			}
			return RecsStatus;
		}

		public async Task GetFullUserData()
		{
			try
			{
				User = await TinderHelper.GetFullUserData();
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
			}
		}

		private bool IsConnected()
		{
			return AuthStatus == AuthStatus.Okay;
		}

		private void AddNewMatch(string message)
		{
			UpdateMatches(this, null);
		}

		private void StartUpdatingMatches()
		{
			DispatcherTimer timerMatches = new DispatcherTimer();
			timerMatches.Tick += UpdateMatches;
			timerMatches.Interval = new TimeSpan(0, 0, 10);
			timerMatches.Start();
		}

		private void StartUpdatingRecs()
		{
			DispatcherTimer timerRecs = new DispatcherTimer();
			timerRecs.Tick += UpdateRecs;
			timerRecs.Interval = new TimeSpan(0, 15, 0);
			timerRecs.Start();
		}

		private async void UpdateRecs(object sender, EventArgs e)
		{
			if (RecList.Count == 0)
			{
				await GetRecs();
			}
		}

		private async void UpdateMatches(object sender, EventArgs e)
		{
			try
			{
				var newUpdates = await TinderHelper.GetUpdates(Properties.Settings.Default.LastUpdate);

				if (newUpdates.Matches.Count != 0)
				{
					Properties.Settings.Default["LastUpdate"] = newUpdates.LastActivityDate;
					Properties.Settings.Default.Save();
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
							UpdatedMatches.Add(matchToUpdate);
							matchToUpdate.LastActivityDate = newMatch.LastActivityDate;
							Messenger.Default.Send("", MessengerToken.SortMatchList);
						}
						// There's a new match
						else
						{
							new Thread(() => SerializationHelper.SerializeMatch(newMatch)).Start();
							MatchList.Insert(0, newMatch);
						}
					}
				}
			}
			catch (TinderRequestException ex)
			{
				MessageBox.Show(ex.Message);
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
				item.Messages = item.Messages ?? new ObservableCollection<MessageModel>();

				item.Messages.CollectionChanged += Messages_CollectionChanged;
			}

			// Remove ghost matches, which I don't know why exist
			for (int i = 0; i < MatchList.Count; i++)
				if (MatchList[i].Person == null)
					MatchList.RemoveAt(i--);

			Messenger.Default.Send("", MessengerToken.SortMatchList);
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

		private void Unmatch(MatchModel match)
		{
			var decision = MessageBox.Show($"Do you really want to unmatch with {match.Person.Name}?",
				"Are you sure about that?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

			if (decision == MessageBoxResult.Yes)
			{
				try
				{
					TinderHelper.UnmatchPerson(match.Id);
					MatchList.Remove(match);
				}
				catch (TinderRequestException e)
				{
					MessageBox.Show(e.Message);
				}
			}
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

		/// <summary>
		/// Opens match profile and forces to download additional data
		/// </summary>
		/// <param name="match"></param>
		private async void OpenMatchProfile(MatchModel match)
		{
			UpdateMatchModel(match, await TinderHelper.GetFullMatchData(match.Person.Id));

			new Thread(() => SerializationHelper.SerializeMatch(match)).Start();

			Messenger.Default.Send(match, MessengerToken.ShowMatchProfile);
		}

		/// <summary>
		/// Updates current match model with new data from match update
		/// </summary>
		/// <param name="match"></param>
		/// <param name="matchUpdate"></param>
		private void UpdateMatchModel(MatchModel match, MatchModel matchUpdate)
		{
			match.Instagram = matchUpdate.Instagram;
			match.DistanceMiles = matchUpdate.DistanceMiles;
			match.SpotifyThemeTrack = matchUpdate.SpotifyThemeTrack;
			match.SpotifyTopArtists = matchUpdate.SpotifyTopArtists;
			match.CommonFriendCount = matchUpdate.CommonFriendCount;
			match.CommonLikeCount = matchUpdate.CommonLikeCount;
			match.CommonFriends = matchUpdate.CommonFriends;
			match.CommonLikes = matchUpdate.CommonLikes;
			match.ConnectionCount = matchUpdate.ConnectionCount;
		}

		private async void ForceDownloadMatches()
		{
			ConnectionStatusEventArgs args = new ConnectionStatusEventArgs();
			args.AuthStatus = AuthStatus.Okay;
			args.MatchesStatus = MatchesStatus.Getting;
			ConnectionStatusChanged.Invoke(this, args);

			await GetMatches();
			Messenger.Default.Send(new SerializationPacket(MatchList), MessengerToken.ShowSerializationDialog);

			args.MatchesStatus = MatchesStatus.Okay;
			ConnectionStatusChanged.Invoke(this, args);

		}

		private async void ForceDownloadRecs()
		{
			ConnectionStatusEventArgs args = new ConnectionStatusEventArgs();
			args.AuthStatus = AuthStatus.Okay;
			args.RecsStatus = RecsStatus.Getting;
			ConnectionStatusChanged.Invoke(this, args);

			await GetRecs();
			Messenger.Default.Send(new SerializationPacket(RecList), MessengerToken.ShowSerializationDialog);

			args.RecsStatus = RecsStatus.Okay;
			ConnectionStatusChanged.Invoke(this, args);
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

		/// <summary>
		/// Event is handled when application get shutdown notice
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Current_Exit(object sender, ExitEventArgs e)
		{
			 SerializationHelper.SerializeMatchList(UpdatedMatches, null);
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