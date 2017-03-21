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
using System.Windows.Data;
using System.Diagnostics;
using Twinder.Properties;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twinder.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
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
			set
			{
				Set(ref _matchList, value);

				MatchListCvs = new CollectionViewSource();
				MatchListCvs.Source = MatchList;
				MatchListCvs.Filter += FilterVM.MatchList_ApplyFilter;
				MatchListCvs.View.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
					FilterVM.FilteredMatchListCount = MatchListCvs.View.Cast<object>().Count();
			}
		}

		private List<MatchModel> _newMatchList;
		public List<MatchModel> NewMatchList
		{
			get { return _newMatchList; }
			set { Set(ref _newMatchList, value); }
		}


		private CollectionViewSource _matchListCvs;
		public CollectionViewSource MatchListCvs
		{
			get { return _matchListCvs; }
			set { Set(ref _matchListCvs, value); }
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

		/// <summary>
		/// Matches are added here when they receive an update or have new messages for serialization
		/// </summary>
		public ObservableCollection<MatchModel> UpdatedMatches { get; private set; }


		// Current connection status
		private string _connectionStatus = Properties.Resources.tinder_auth_connecting;
		public string ConnectionStatus
		{
			get { return _connectionStatus; }
			set
			{
				Set(ref _connectionStatus, value);
				if (value == Properties.Resources.tinder_auth_okay || value == Properties.Resources.tinder_auth_error)
					IsConnecting = false;
				else
					IsConnecting = true;
			}
		}

		// Whether to show progress bar or not based on current connection status
		private bool _isConnecting = true;
		public bool IsConnecting
		{
			get { return _isConnecting; }
			set { Set(ref _isConnecting, value); }
		}

		// Whether to show progress bar or not based on current connection status
		private bool _isConnected = true;
		public bool IsConnected
		{
			get { return _isConnected; }
			set { Set(ref _isConnected, value); }
		}

		// Holds reference to my view to subsribe to events
		public MainWindow MyView { get; set; }

		public RelayCommand<MatchModel> OpenChatCommand { get; private set; }
		public RelayCommand<MatchModel> UnmatchCommand { get; private set; }
		public RelayCommand<MatchModel> DownloadMatchDataCommand { get; private set; }
		public RelayCommand<MatchModel> OpenFolderCommand { get; private set; }
		public RelayCommand<MatchModel> OpenMatchProfileCommand { get; private set; }

		public RelayCommand OpenRecsCommand { get; private set; }
		public RelayCommand OpenUserProfileCommand { get; private set; }
		public RelayCommand SetLocationCommand { get; private set; }
		public RelayCommand ExitCommand { get; private set; }
		public RelayCommand SwitchAccountCommand { get; private set; }
		public RelayCommand AboutCommand { get; private set; }
		public RelayCommand ForceDownloadMatchesCommand { get; private set; }
		public RelayCommand ForceDownloadMatchesFullCommand { get; private set; }
		public RelayCommand ForceDownloadRecsCommand { get; private set; }
		public RelayCommand ClearSearchCommand { get; private set; }

		public MatchListFilterViewModel FilterVM { get; set; }
		public string FbId { get; internal set; }
		public string FbToken { get; internal set; }
		public string UserName { get; internal set; }
		public object SetAsDefault { get; internal set; }

		public MainViewModel()
		{
			InitializeProps();
		}
		private void InitializeProps()
		{
			FilterVM = new MatchListFilterViewModel(this);

			NewMatchList = new List<MatchModel>();
			UpdatedMatches = new ObservableCollection<MatchModel>();

			OpenChatCommand = new RelayCommand<MatchModel>((param) => OpenChat(param));
			OpenMatchProfileCommand = new RelayCommand<MatchModel>(param => OpenMatchProfile(param));
			DownloadMatchDataCommand = new RelayCommand<MatchModel>(param => DownloadFullMatchData(param));
			OpenFolderCommand = new RelayCommand<MatchModel>(param => OpenFolder(param));
			UnmatchCommand = new RelayCommand<MatchModel>(param => Unmatch(param));
			ClearSearchCommand = new RelayCommand(() => FilterVM.NameFilter = string.Empty);

			OpenRecsCommand = new RelayCommand(OpenRecs, () =>
			{
				return IsConnected && RecList != null && RecList.Count != 0;
			});

			OpenUserProfileCommand = new RelayCommand(OpenUserProfile, () => IsConnected);
			SetLocationCommand = new RelayCommand(SetLocation, () => IsConnected);

			ExitCommand = new RelayCommand(Exit);

			SwitchAccountCommand = new RelayCommand(SwitchAccount);
			AboutCommand = new RelayCommand(About);

			ForceDownloadMatchesCommand = new RelayCommand(ForceDownloadMatches);
			ForceDownloadMatchesFullCommand = new RelayCommand(ForceDownloadMatchesFull);
			ForceDownloadRecsCommand = new RelayCommand(ForceDownloadRecs);

			Messenger.Default.Register<string>(this, MessengerToken.ForceUpdate, AddNewMatch);
			Messenger.Default.Register<string>(this, MessengerToken.GetMoreRecs, (input) => UpdateRecs(this, null));

			Application.Current.Exit += Current_Exit;
		}

		/// <summary>
		/// First thing called. Entry point of some sort. Calls other methods to get Tinder data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public async void StartInitialize(object sender, EventArgs e)
		{
			ConnectionStatus = Resources.tinder_auth_connecting;

			// Only do full authorization if it's the first time or data is not saved yet
			if (string.IsNullOrEmpty(UserName))
			{
				if (await Authenticate(FbId, FbToken))
				{

					UserName = User.ToString();

					string lat = "0";
					string lon = "0";

					if (User.Pos != null)
					{
						lat = User.Pos.Latitude.Replace('.', ',');
						lon = User.Pos.Longtitude.Replace('.', ',');
					}

					SerializationHelper.CreateUser(FbId, FbToken, UserName, User.LatestUpdateDate,
						lat, lon);
					SerializationHelper.UpdateTinderToken(Auth.Token);


					// Gets recs
					await GetMatches();

					// Gets recs
					await GetRecs();

					int time = MatchList.Count * 3 / 60;
					// Dont ask just save nobody cares what user wants
					MessageBox.Show($"All your matches will be saved. It may take up to {time} minutes "
						+ "for the download to complete. I recommend not to cancel this action.",
						"Downloading data", MessageBoxButton.OK, MessageBoxImage.Information);

					Messenger.Default.Send(new SerializationPacket(MatchList, RecList, User),
							MessengerToken.ShowSerializationDialog);

					Settings.Default["FirstStart"] = false;
					Settings.Default.Save();

					StartUpdatingMatches();
					StartUpdatingRecs();
					ConnectionStatus = Resources.tinder_auth_okay;
				}

			}
			// It's not the first time launching application
			else
			{
				SerializationHelper.CurrentUser = UserName;


				// FIXME hangs application for a second
				// Deserializes matches and recs first
				MatchList = SerializationHelper.DeserializeMatchList();


				MatchListSetup();
				FilterVM.UpdateStatusBar();

				RecList = SerializationHelper.DeserializeRecList();

				if (await Authenticate(FbId, FbToken))
				{
					string lat = "0";
					string lon = "0";

					if (User.Pos != null)
					{
						lat = User.Pos.Latitude.Replace('.', ',');
						lon = User.Pos.Longtitude.Replace('.', ',');
						SerializationHelper.UpdateUserPosition(lat, lon);
					}


					SerializationHelper.UpdateTinderToken(Auth.Token);

					// Updates last five matches
					Parallel.ForEach(MatchList.OrderByDescending(x => x.LastActivityDate).Take(5), async x =>
					{
						try
						{
							var updatedMatch = await TinderHelper.GetFullMatchData(x.Person.Id);
							SerializationHelper.UpdateMatchModel(x, updatedMatch);
							new Task(() => SerializationHelper.SerializeMatch(x)).Start();
						}
						catch (TinderRequestException ex)
						{
							MessageBox.Show(ex.Message);
						}
					});

					FilterVM.SortMatchList();

					UpdateMatches(this, null);
					UpdateRecs(this, null);

					ConnectionStatus = Resources.tinder_auth_okay;

					// Starts automatic updates
					StartUpdatingMatches();
					StartUpdatingRecs();
				}
				else
					ConnectionStatus = Resources.tinder_auth_error;

			}

			// Now we check if there's a new version available
			var restClient = new RestClient("https://api.github.com/");
			var request = new RestRequest("repos/dainius14/twinder/releases/latest", Method.GET);
			var response = await restClient.ExecuteTaskAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var json = JsonConvert.DeserializeObject<dynamic>(response.Content);
				if (!("v" + Assembly.GetExecutingAssembly().GetName().Version.ToString()).StartsWith((string)json.tag_name))
				{
					Messenger.Default.Send((string)json.html_url, MessengerToken.ShowUpdateAvailableDialog);
				}
			}


		}

		/// <summary>
		/// Tries authenticating with Tinder servers and getting User Data
		/// </summary>
		/// <returns></returns>
		public async Task<bool> Authenticate(string fbId, string fbToken)
		{
			try
			{
				// We get tinder token everytime
				Auth = await TinderHelper.GetAuthData(fbId, fbToken);
				User = await TinderHelper.GetFullUserData();

				return true;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Tries connecting to Tinder servers and getting updates
		/// </summary>
		/// <returns></returns>
		public async Task<bool> GetMatches()
		{
			try
			{
				Updates = await TinderHelper.GetUpdates();

				if (MatchList == null)
					MatchList = Updates.Matches;

				MatchListSetup();

				SerializationHelper.UpdateLastUpdate(Updates.LastActivityDate);
				return true;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Tries connecting to Tinder servers and getting recommendations
		/// </summary>
		/// <returns></returns>
		public async Task<bool> GetRecs()
		{
			try
			{
				var recs = await TinderHelper.GetRecommendations();
				if (recs.Recommendations != null)
				{
					// Out of recs
					if (recs.Recommendations.Any(x => x.Id.Contains("tinder_rate_limited")))
					{
						RecList.Clear();
						SerializationHelper.EmptyRecommendations();
						return false;
					}
					// If it's the first time getting recs
					if (RecList == null)
						RecList = new ObservableCollection<RecModel>(recs.Recommendations);
					// Only useful if we force to download new recs in which case old
					// recs would be no use anyway
					RecList.Clear();
					foreach (var item in recs.Recommendations)
						RecList.Add(item);

					return true;
				}
				else
					return false;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
				return false;
			}
		}

		private async void GetMoreRecs(string obj)
		{
			await GetRecs();
			Messenger.Default.Send(new SerializationPacket(RecList));
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
			// There are no matches from before, try to get more
			if (RecList.Count == 0)
			{
				await GetRecs();

				// If there are new matches, show dialog
				if (RecList.Count != 0)
					Messenger.Default.Send(new SerializationPacket(RecList),
							MessengerToken.ShowSerializationDialog);
			}
		}

		private async void UpdateMatches(object sender, EventArgs e)
		{
			try
			{
				var newUpdates = await TinderHelper.GetUpdates(SerializationHelper.GetLastUpdate());

				if (newUpdates.Matches.Count != 0)
				{
					SerializationHelper.UpdateLastUpdate(newUpdates.LastActivityDate);

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

							if (!UpdatedMatches.Contains(matchToUpdate))
								UpdatedMatches.Add(matchToUpdate);

							matchToUpdate.LastActivityDate = newMatch.LastActivityDate;

							new Task(() => SerializationHelper.SerializeMatch(matchToUpdate)).Start();


						}
						// There's a new match
						else
						{
							new Task(() => SerializationHelper.SerializeMatch(newMatch)).Start();
							MatchList.Insert(0, newMatch);
							NewMatchList.Add(newMatch);
						}
					}
				}

				if (newUpdates.Blocks.Count != 0)
				{
					foreach (var unmatched in newUpdates.Blocks)
					{
						var match = MatchList.Where(x => x.Id == unmatched).FirstOrDefault();
						if (match != null)
						{
							SerializationHelper.MoveMatchToUnMatched(match);
							MatchList.Remove(match);
							MessageBox.Show(match.Person.Name + " unmatched you");
						}

					}

				}

				FilterVM.SortMatchList();
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

				// For when there are already handlers attached
				item.Messages.CollectionChanged -= Messages_CollectionChanged;
				item.Messages.CollectionChanged += Messages_CollectionChanged;
			}

			// Remove ghost matches, which I don't know why exist
			for (int i = 0; i < MatchList.Count; i++)
				if (MatchList[i].Person == null)
					MatchList.RemoveAt(i--);

			FilterVM.SortMatchList();
		}

		/// <summary>
		/// If there are new messages added to collection, updates the view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			FilterVM.SortMatchList();
		}

		private async void DownloadFullMatchData(MatchModel param)
		{
			ConnectionStatus = Properties.Resources.tinder_update_getting_matches;

			var updatedMatch = await TinderHelper.GetFullMatchData(param.Person.Id);
			SerializationHelper.UpdateMatchModel(param, updatedMatch);
			new Task(() => SerializationHelper.SerializeMatch(param)).Start();

			ConnectionStatus = Properties.Resources.tinder_auth_okay;
			FilterVM.SortMatchList();
		}

		private void Unmatch(MatchModel match)
		{
			var decision = MessageBox.Show($"Do you really want to unmatch with {match.Person.Name}?",
				"Are you sure about that?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

			if (decision == MessageBoxResult.Yes)
			{
				try
				{
					SerializationHelper.MoveMatchToUnMatchedByMe(match);
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

			// Adding match to UpdateMatches list is the least complex way to force serialization
			// when user sends messages
			if (!UpdatedMatches.Contains(match))
				UpdatedMatches.Add(match);
		}

		/// <summary>
		/// Opens match profile and forces to download additional data
		/// </summary>
		/// <param name="match"></param>
		private void OpenMatchProfile(MatchModel match)
		{
			Messenger.Default.Send(match, MessengerToken.ShowMatchProfile);

			//DownloadFullMatchData(match);
		}

		private void OpenFolder(MatchModel param)
		{
			Process.Start(SerializationHelper.GetMatchFolder(param));
		}

		/// <summary>
		/// Download all updates of matches
		/// </summary>
		private async void ForceDownloadMatches()
		{
			ConnectionStatus = Properties.Resources.tinder_update_getting_matches;
			try
			{
				bool setUpMatchList = false;
				var updates = await TinderHelper.GetUpdates();

				// FIXME For 200 matches this hangs for ~10s
				foreach (var item in updates.Matches.Where(x => x.Person != null))
				{
					var match = MatchList.FirstOrDefault(x => x.Person.Id == item.Person.Id);
					if (match != null)
						SerializationHelper.UpdateMatchModel(match, item);
					else
					{
						MatchList.Add(item);
						setUpMatchList = true;
					}
				}

				if (setUpMatchList)
					MatchListSetup();

				Messenger.Default.Send(new SerializationPacket(MatchList), MessengerToken.ShowSerializationDialog);

				ConnectionStatus = Properties.Resources.tinder_auth_okay;

			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
			}
		}

		/// <summary>
		/// Downloads full data of every match
		/// </summary>
		private void ForceDownloadMatchesFull()
		{
			Messenger.Default.Send(new SerializationPacket(MatchList, true), MessengerToken.ShowSerializationDialog);
		}

		/// <summary>
		/// Downloads all recommendations
		/// </summary>
		private void ForceDownloadRecs()
		{
			if (SerializationHelper.EmptyRecommendations())
			{
				RecList.Clear();
				UpdateRecs(this, null);
			}
		}

		private void SwitchAccount()
		{
			// Loads another instance of this application with command args
			Process.Start(Application.ResourceAssembly.Location, App.ARG_SHOW_ACCOUNT_SWITCHER);
		}

		private void About()
		{
			string appName = Properties.Resources.app_title;
			string version = "Version " + Assembly.GetEntryAssembly().GetName().Version.ToString(3);
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
		/// Serializes match list on shutdown
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
		public ConnectionStatuss ConnectionStatus { get; set; }

		public ConnectionStatusEventArgs(ConnectionStatuss connectionStatus)
		{
			ConnectionStatus = connectionStatus;
		}
	}

	public enum ConnectionStatuss
	{
		Okay,
		Authenticating,
		GettingMatchesAndRecs,
		GettingMatches,
		GettingRecs,
		Error
	}

	public enum DescriptionFilter
	{
		Both = 0,
		WithDescription,
		WithoutDescription
	}

	public enum MessagedFilter
	{
		All = 0,
		Messaged,
		NotMessaged
	}
}