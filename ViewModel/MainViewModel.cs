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
using System.Windows.Data;
using System.Text.RegularExpressions;

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
			set
			{
				Set(ref _matchList, value);

				MatchListCvs = new CollectionViewSource();
				MatchListCvs.Source = MatchList;
				MatchListCvs.Filter += MatchList_ApplyFilter;
				MatchListCvs.View.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) =>
					FilteredMatchListCount = MatchListCvs.View.Cast<object>().Count();
			}
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

		public AuthStatus AuthStatus { get; internal set; }
		public MatchesStatus MatchesStatus { get; internal set; }
		public RecsStatus RecsStatus { get; internal set; }

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

		public RelayCommand ShowMoreFilteringCommand { get; private set; }

		private CollectionViewSource _matchListCvs;
		public CollectionViewSource MatchListCvs
		{
			get { return _matchListCvs; }
			set { Set(ref _matchListCvs, value); }
		}

		private int _filteredMatchListCount;
		public int FilteredMatchListCount
		{
			get { return _filteredMatchListCount; }
			set { Set(ref _filteredMatchListCount, value); }
		}


		private bool _showMoreFiltering;
		public bool ShowMoreFiltering
		{
			get { return _showMoreFiltering; }
			set
			{
				Set(ref _showMoreFiltering, value);
			}
		}

		private string _nameFilter;
		public string NameFilter
		{
			get { return _nameFilter; }
			set
			{
				Set(ref _nameFilter, value);
				MatchListCvs.View.Refresh();
			}
		}

		private int? _minAgeFilter;
		public int? MinAgeFilter
		{
			get { return _minAgeFilter; }
			set
			{
				Set(ref _minAgeFilter, value);
				MatchListCvs.View.Refresh();
			}
		}

		private int? _maxAgeFilter;
		public int? MaxAgeFilter
		{
			get { return _maxAgeFilter; }
			set
			{
				Set(ref _maxAgeFilter, value);
				MatchListCvs.View.Refresh();
			}
		}


		private DescriptionFilter _descriptionFilter;
		public DescriptionFilter DescriptionFilter
		{
			get { return _descriptionFilter; }
			set
			{
				Set(ref _descriptionFilter, value);
				MatchListCvs.View.Refresh();
			}
		}

		private string _descriptionWordFilter;
		public string DescriptionWordFilter
		{
			get { return _descriptionWordFilter; }
			set
			{
				Set(ref _descriptionWordFilter, value);
				MatchListCvs.View.Refresh();
			}
		}

		private bool _descriptionWholeWordsFilter = false;
		public bool DescriptionWholeWordsFilter
		{
			get { return _descriptionWholeWordsFilter; }
			set
			{
				Set(ref _descriptionWholeWordsFilter, value);
				MatchListCvs.View.Refresh();
			}
		}


		private string _messagesWordFilter;
		public string MessagesWordFilter
		{
			get { return _messagesWordFilter; }
			set
			{
				Set(ref _messagesWordFilter, value);
				MatchListCvs.View.Refresh();
			}
		}

		private bool _messagesWholeWordsFilter = false;
		public bool MessagesWholeWordsFilter
		{
			get { return _messagesWholeWordsFilter; }
			set
			{
				Set(ref _messagesWholeWordsFilter, value);
				MatchListCvs.View.Refresh();
			}
		}


		private int? _minMessagesFilter;
		public int? MinMessagesFilter
		{
			get { return _minMessagesFilter; }
			set
			{
				Set(ref _minMessagesFilter, value);
				MatchListCvs.View.Refresh();
			}
		}

		private int? _maxMessagesFilter;
		public int? MaxMessagesFilter
		{
			get { return _maxMessagesFilter; }
			set
			{
				Set(ref _maxMessagesFilter, value);
				MatchListCvs.View.Refresh();
			}
		}


		private Gender _genderFilter = Gender.Both;
		public Gender GenderFilter
		{
			get { return _genderFilter; }
			set
			{
				Set(ref _genderFilter, value);
				MatchListCvs.View.Refresh();
			}
		}


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

			ShowMoreFilteringCommand = new RelayCommand(() => ShowMoreFiltering = !ShowMoreFiltering);

		}

		private void MatchList_ApplyFilter(object sender, FilterEventArgs e)
		{
			var match = (MatchModel) e.Item;

			bool isNameAccepted;
			if (string.IsNullOrWhiteSpace(NameFilter))
				isNameAccepted = true;
			else
				isNameAccepted = match.Person.Name.ToLower().StartsWith(NameFilter.ToLower());


			bool isDescriptionAccepted;
			if (DescriptionFilter == DescriptionFilter.Both)
				isDescriptionAccepted = true;
			else if (DescriptionFilter == DescriptionFilter.WithDescription)
				isDescriptionAccepted = !string.IsNullOrWhiteSpace(match.Person.Bio);
			else
				isDescriptionAccepted = string.IsNullOrWhiteSpace(match.Person.Bio);


			bool isDescriptionWordsAccepted;
			if (!string.IsNullOrWhiteSpace(DescriptionWordFilter))
			{

				isDescriptionWordsAccepted = false;
				string regex;
				if (DescriptionWholeWordsFilter)
					regex = "\\b" + DescriptionWordFilter.Replace(" ", "\\b|\\b") + "\\b";
				else
					regex = DescriptionWordFilter.Replace(' ', '|');

				isDescriptionWordsAccepted = Regex.Match(match.Person.Bio, regex, RegexOptions.IgnoreCase).Success;

			}
			else
				isDescriptionWordsAccepted = true;

			// Age
			bool isMinAgeAccepted;
			if (MinAgeFilter != null)
			{
				if (match.Person.Age >= MinAgeFilter)
					isMinAgeAccepted = true;
				else
					isMinAgeAccepted = false;
			}
			else
				isMinAgeAccepted = true;

			bool isMaxAgeAccepted;
			if (MaxAgeFilter != null)
			{
				if (match.Person.Age <= MaxAgeFilter)
					isMaxAgeAccepted = true;
				else
					isMaxAgeAccepted = false;
			}
			else
				isMaxAgeAccepted = true;

			// Message count
			bool isMinMessagesAccepted;
			if (MinMessagesFilter != null)
			{
				if (match.Messages.Count >= MinMessagesFilter)
					isMinMessagesAccepted = true;
				else
					isMinMessagesAccepted = false;
			}
			else
				isMinMessagesAccepted = true;

			bool isMaxMessagesAccepted;
			if (MaxMessagesFilter != null)
			{
				if (match.Messages.Count <= MaxMessagesFilter)
					isMaxMessagesAccepted = true;
				else
					isMaxMessagesAccepted = false;
			}
			else
				isMaxMessagesAccepted = true;

			// Messages include
			bool isMessagesWordAccepted;
			if (!string.IsNullOrWhiteSpace(MessagesWordFilter))
			{
				isMessagesWordAccepted = false;
				string regex;
				if (MessagesWholeWordsFilter)
					regex = "\\b" + MessagesWordFilter.Replace(" ", "\\b|\\b") + "\\b";
				else
					regex = MessagesWordFilter.Replace(' ', '|');

				isMessagesWordAccepted = match.Messages.Any
					(x => Regex.Match(x.Message, regex, RegexOptions.IgnoreCase).Success);
				
			}
			else
				isMessagesWordAccepted = true;

			// Gender
			bool isGenderAccepted;
			if (GenderFilter == Gender.Both)
				isGenderAccepted = true;
			else
				isGenderAccepted = (Gender) Enum.Parse(typeof(Gender), match.Person.Gender.ToString()) == GenderFilter;
			
			e.Accepted = isNameAccepted && isDescriptionAccepted && isDescriptionWordsAccepted
				&& isMinAgeAccepted && isMaxAgeAccepted
				&& isMinMessagesAccepted && isMaxMessagesAccepted && isMessagesWordAccepted
				&& isGenderAccepted;
		}

		private async void GetMoreRecs(string obj)
		{
			await GetRecs();
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
					//var result = MessageBox.Show($"Do you want to save all match data? It may take up to {time} minutes.",
					//	"Saved data not found", MessageBoxButton.YesNo, MessageBoxImage.Question);
					//if (result == MessageBoxResult.Yes)
					//	Messenger.Default.Send(new SerializationPacket(MatchList, RecList, User),
					//		MessengerToken.ShowSerializationDialog);
					// Dont ask just save, 

					MessageBox.Show($"All your matches will be saved. It may take up to {time} minutes"
						+ "for the download to complete. I recommend not to cancel this action.",
						"Downloading data", MessageBoxButton.OK, MessageBoxImage.Information);

					Messenger.Default.Send(new SerializationPacket(MatchList, RecList, User),
							MessengerToken.ShowSerializationDialog);

					Properties.Settings.Default["FirstStart"] = false;
					Properties.Settings.Default.Save();

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

				// If doing complete update, we only want to add new, so we don't lose bindings n shit
				if (MatchList == null)
					MatchList = Updates.Matches;
				else
				{
					MatchList.Clear();

					foreach (var newItem in Updates.Matches)
						MatchList.Add(newItem);
				}

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
					// Either we force a new reclist or rec list was empty
					else
					{
						RecList.Clear();
						foreach (var item in recs.Recommendations)
							RecList.Add(item);
					}

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

				if (newUpdates.Blocks.Count != 0)
				{

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
		}

		/// <summary>
		/// Opens match profile and forces to download additional data
		/// </summary>
		/// <param name="match"></param>
		private async void OpenMatchProfile(MatchModel match)
		{
			Messenger.Default.Send(match, MessengerToken.ShowMatchProfile);

			var updatedMatch = await TinderHelper.GetFullMatchData(match.Person.Id);

			UpdateMatchModel(match, updatedMatch);
			new Thread(() => SerializationHelper.SerializeMatch(match)).Start();


			Messenger.Default.Send("", MessengerToken.RefreshMatchList);

		}

		/// <summary>
		/// Updates current match model with new data from match update
		/// </summary>
		/// <param name="match"></param>
		/// <param name="matchUpdate"></param>
		private void UpdateMatchModel(MatchModel match, MatchUpdateResultsModel matchUpdate)
		{
			match.Person.Bio = matchUpdate.Bio;
			match.Person.BirthDate = matchUpdate.BirthDate;
			match.Person.PingTime = matchUpdate.PingTime;

			match.Person.Photos.Clear();
			foreach (var item in matchUpdate.Photos)
				match.Person.Photos.Add(item);

			match.Person.Photos = matchUpdate.Photos;

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

			SerializationHelper.EmptyRecommendations();
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

	public enum DescriptionFilter
	{
		Both = 0,
		WithDescription,
		WithoutDescription
	}
}