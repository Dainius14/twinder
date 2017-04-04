using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using Twinder.Helpers;
using Twinder.Model;

namespace Twinder.ViewModel
{
	public class MatchListFilterViewModel : ViewModelBase
	{
		/// <summary>
		/// Holds reference to main view model to access needed things
		/// </summary>
		public MainViewModel MainVM { get; private set; }


		private TabItem _selectedTab;
		public TabItem SelectedTab
		{
			get { return _selectedTab; }
			set
			{
				Set(ref _selectedTab, value);
				UpdateStatusBar();
			}
		}

		private int _selectedCount;
		public int SelectedCount
		{
			get { return _selectedCount; }
			set { Set(ref _selectedCount, value); }
		}

		private int _filteredCount;
		public int FilteredCount
		{
			get { return _filteredCount; }
			set { Set(ref _filteredCount, value); }
		}


		private int _filteredMatchListCount;
		public int FilteredMatchListCount
		{
			get { return _filteredMatchListCount; }
			set { Set(ref _filteredMatchListCount, value); }
		}

		private int _filteredUnmatchMeListCount;
		public int FilteredUnmatchedMeListCount
		{
			get { return _filteredUnmatchMeListCount; }
			set { Set(ref _filteredUnmatchMeListCount, value); }
		}

		private int _filteredUnmatchByMeListCount;
		public int FilteredUnmatchedByMeListCount
		{
			get { return _filteredUnmatchByMeListCount; }
			set { Set(ref _filteredUnmatchByMeListCount, value); }
		}

		private int _filteredRecommendationsPendingCount;
		public int FilteredRecommendationsPendingListCount
		{
			get { return _filteredRecommendationsPendingCount; }
			set { Set(ref _filteredRecommendationsPendingCount, value); }
		}

		private int _filteredRecommendationsPassedCount;
		public int FilteredRecommendationsPassedListCount
		{
			get { return _filteredRecommendationsPassedCount; }
			set { Set(ref _filteredRecommendationsPassedCount, value); }
		}


		private bool _showMoreFiltering = false;
		public bool ShowMoreFiltering
		{
			get { return _showMoreFiltering; }
			set { Set(ref _showMoreFiltering, value); }
		}

		private string _nameFilter;
		public string NameFilter
		{
			get { return _nameFilter; }
			set
			{
				Set(ref _nameFilter, value);
				RefreshCvs();
			}
		}

		private int? _minAgeFilter;
		public int? MinAgeFilter
		{
			get { return _minAgeFilter; }
			set
			{
				Set(ref _minAgeFilter, value);
				RefreshCvs();
			}
		}

		private int? _maxAgeFilter;
		public int? MaxAgeFilter
		{
			get { return _maxAgeFilter; }
			set
			{
				Set(ref _maxAgeFilter, value);
				RefreshCvs();
			}
		}


		private DescriptionFilter _descriptionFilter;
		public DescriptionFilter DescriptionFilter
		{
			get { return _descriptionFilter; }
			set
			{
				Set(ref _descriptionFilter, value);
				RefreshCvs();
			}
		}

		private string _descriptionWordFilter;
		public string DescriptionWordFilter
		{
			get { return _descriptionWordFilter; }
			set
			{
				Set(ref _descriptionWordFilter, value);
				RefreshCvs();
			}
		}

		private bool _descriptionWholeWordsFilter = false;
		public bool DescriptionWholeWordsFilter
		{
			get { return _descriptionWholeWordsFilter; }
			set
			{
				Set(ref _descriptionWholeWordsFilter, value);
				RefreshCvs();
			}
		}


		private string _messagesWordFilter;
		public string MessagesWordFilter
		{
			get { return _messagesWordFilter; }
			set
			{
				Set(ref _messagesWordFilter, value);
				RefreshCvs();
			}
		}

		private bool _messagesWholeWordsFilter = false;
		public bool MessagesWholeWordsFilter
		{
			get { return _messagesWholeWordsFilter; }
			set
			{
				Set(ref _messagesWholeWordsFilter, value);
				RefreshCvs();
			}
		}


		private int? _minMessagesFilter;
		public int? MinMessagesFilter
		{
			get { return _minMessagesFilter; }
			set
			{
				Set(ref _minMessagesFilter, value);
				RefreshCvs();
			}
		}

		private int? _maxMessagesFilter;
		public int? MaxMessagesFilter
		{
			get { return _maxMessagesFilter; }
			set
			{
				Set(ref _maxMessagesFilter, value);
				RefreshCvs();
			}
		}

		private Gender _genderFilter = Gender.Both;
		public Gender GenderFilter
		{
			get { return _genderFilter; }
			set
			{
				Set(ref _genderFilter, value);
				RefreshCvs();
			}
		}


		private MessagedFilter _messagedFilter = MessagedFilter.All;
		public MessagedFilter MessagedFilter
		{
			get { return _messagedFilter; }
			set
			{
				Set(ref _messagedFilter, value);
				RefreshCvs();
			}
		}

		private bool _onlyNewMatches = false;
		public bool OnlyNewMatches
		{
			get { return _onlyNewMatches; }
			set
			{
				Set(ref _onlyNewMatches, value);
				RefreshCvs();
			}
		}

		// Sorting
		public IEnumerable<SortingOptionsEnum> SortingOptions
		{
			get { return Enum.GetValues(typeof(SortingOptionsEnum)).Cast<SortingOptionsEnum>(); }
		}

		private SortingOptionsEnum _selectedSorting = SortingOptionsEnum.ActivityDesc;
		public SortingOptionsEnum SelectedSorting
		{
			get { return _selectedSorting; }
			set
			{
				Set(ref _selectedSorting, value);
				SortMatchList();
			}
		}

		public RelayCommand ShowMoreFilteringCommand { get; private set; }
		public RelayCommand ResetFilterCommand { get; private set; }


		public MatchListFilterViewModel(MainViewModel mainViewModel)
		{
			MainVM = mainViewModel;

			ShowMoreFilteringCommand = new RelayCommand(() => ShowMoreFiltering = !ShowMoreFiltering);
			ResetFilterCommand = new RelayCommand(() =>
			{
				NameFilter = string.Empty;

				DescriptionFilter = DescriptionFilter.Both;
				DescriptionWordFilter = string.Empty;
				DescriptionWholeWordsFilter = false;

				MinMessagesFilter = null;
				MaxMessagesFilter = null;
				MessagesWordFilter = string.Empty;
				MessagesWholeWordsFilter = false;

				MinAgeFilter = null;
				MaxAgeFilter = null;
				GenderFilter = Gender.Both;
			});
		}

		private void RefreshCvs()
		{
			MainVM.MatchListCvs.View.Refresh();
			if (MainVM.UnmatchedMeListCvs != null)
				MainVM.UnmatchedMeListCvs.View.Refresh();
			if (MainVM.UnmatchedByMeListCvs != null)
				MainVM.UnmatchedByMeListCvs.View.Refresh();
			if (MainVM.RecommendationsPendingCvs != null)
				MainVM.RecommendationsPendingCvs.View.Refresh();
			if (MainVM.RecommendationsPassedCvs != null)
				MainVM.RecommendationsPassedCvs.View.Refresh();

			UpdateStatusBar();
		}

		/// <summary>
		/// Sort match list collection with given sorting option
		/// </summary>
		public void SortMatchList()
		{
			SortDescription sortDescription = new SortDescription();
			switch (SelectedSorting)
			{
				case SortingOptionsEnum.ActivityAsc:
					sortDescription.PropertyName = nameof(MatchModel.LastActivityDate);
					sortDescription.Direction = ListSortDirection.Ascending;
					break;
				case SortingOptionsEnum.ActivityDesc:
					sortDescription.PropertyName = nameof(MatchModel.LastActivityDate);
					sortDescription.Direction = ListSortDirection.Descending;
					break;
				case SortingOptionsEnum.NameAsc:
					sortDescription.PropertyName = nameof(MatchModel.Person) + "." + nameof(PersonModel.Name);
					sortDescription.Direction = ListSortDirection.Ascending;
					break;
				case SortingOptionsEnum.NameDesc:
					sortDescription.PropertyName = nameof(MatchModel.Person) + "." + nameof(PersonModel.Name);
					sortDescription.Direction = ListSortDirection.Descending;
					break;
				case SortingOptionsEnum.MatchedAsc:
					sortDescription.PropertyName = nameof(MatchModel.CreatedDate);
					sortDescription.Direction = ListSortDirection.Ascending;
					break;
				case SortingOptionsEnum.MatchedDesc:
					sortDescription.PropertyName = nameof(MatchModel.CreatedDate);
					sortDescription.Direction = ListSortDirection.Descending;
					break;
			}

			using (MainVM.MatchListCvs.DeferRefresh())
			{
				MainVM.MatchListCvs.SortDescriptions.Clear();
				MainVM.MatchListCvs.SortDescriptions.Add(sortDescription);
			}

			if (MainVM.UnmatchedMeList != null)
				using (MainVM.UnmatchedMeListCvs.DeferRefresh())
				{
					MainVM.UnmatchedMeListCvs.SortDescriptions.Clear();
					MainVM.UnmatchedMeListCvs.SortDescriptions.Add(sortDescription);
				}

			if (MainVM.UnmatchedByMeList != null)
				using (MainVM.UnmatchedMeListCvs.DeferRefresh())
				{
					MainVM.UnmatchedByMeListCvs.SortDescriptions.Clear();
					MainVM.UnmatchedByMeListCvs.SortDescriptions.Add(sortDescription);
				}

			if (MainVM.RecommendationsPendingList != null)
				using (MainVM.RecommendationsPendingCvs.DeferRefresh())
				{
					MainVM.RecommendationsPendingCvs.SortDescriptions.Clear();
					MainVM.RecommendationsPendingCvs.SortDescriptions.Add(sortDescription);
				}

			if (MainVM.RecommendationsPassedList != null)
				using (MainVM.RecommendationsPassedCvs.DeferRefresh())
				{
					MainVM.RecommendationsPassedCvs.SortDescriptions.Clear();
					MainVM.RecommendationsPassedCvs.SortDescriptions.Add(sortDescription);
				}
		}

		internal void ApplyFilter(object sender, FilterEventArgs e)
		{
			string name, bio;
			int age;
			Gender gender;
			if (e.Item is MatchModel)
			{
				var match = (MatchModel) e.Item;
				name = match.Person.Name;
				bio = match.Person.Bio;
				age = match.Person.Age;
				gender = (Gender) Enum.Parse(typeof(Gender), match.Person.Gender.ToString());
			}
			else if (e.Item is RecModel)
			{
				var rec = (RecModel) e.Item;
				name = rec.Name;
				bio = rec.Bio;
				age = rec.Age;
				gender = (Gender) Enum.Parse(typeof(Gender), rec.Gender.ToString());
			}
			else
				throw new ArgumentException("Woah wtf happened");

			// Name
			bool isNameAccepted = true;
			if (!string.IsNullOrWhiteSpace(NameFilter))
			{
				isNameAccepted = name.ToLower().StartsWith(NameFilter.ToLower());
			}

			// Description is null or not
			bool isDescriptionAccepted = true;
			if (DescriptionFilter == DescriptionFilter.WithDescription)
			{
				isDescriptionAccepted = !string.IsNullOrWhiteSpace(bio);
			}
			else if (DescriptionFilter == DescriptionFilter.WithoutDescription)
				isDescriptionAccepted = string.IsNullOrWhiteSpace(bio);

			// Description words
			bool isDescriptionWordsAccepted = true;
			if (!string.IsNullOrWhiteSpace(DescriptionWordFilter))
			{
				isDescriptionWordsAccepted = false;
				string regex;
				if (DescriptionWholeWordsFilter)
					regex = "\\b" + DescriptionWordFilter.Replace(" ", "\\b|\\b") + "\\b";
				else
					regex = DescriptionWordFilter.Replace(' ', '|');

				try
				{
					isDescriptionWordsAccepted = Regex.Match(bio, regex, RegexOptions.IgnoreCase).Success;
				}
				catch (ArgumentException)
				{
					// TODO escape special symbols
				}
			}

			// Age
			bool isMinAgeAccepted = true;
			if (MinAgeFilter != null)
			{
				if (age >= MinAgeFilter)
					isMinAgeAccepted = true;
				else
					isMinAgeAccepted = false;
			}

			bool isMaxAgeAccepted = true;
			if (MaxAgeFilter != null)
			{
				if (age <= MaxAgeFilter)
					isMaxAgeAccepted = true;
				else
					isMaxAgeAccepted = false;
			}

			// Message count
			bool isMinMessagesAccepted = true;
			if (MinMessagesFilter != null && e.Item is MatchModel)
			{
				if (((MatchModel) e.Item).Messages.Count >= MinMessagesFilter)
					isMinMessagesAccepted = true;
				else
					isMinMessagesAccepted = false;
			}

			bool isMaxMessagesAccepted = true;
			if (MaxMessagesFilter != null && e.Item is MatchModel)
			{
				if (((MatchModel) e.Item).Messages.Count <= MaxMessagesFilter)
					isMaxMessagesAccepted = true;
				else
					isMaxMessagesAccepted = false;
			}

			// Messages include
			bool isMessagesWordAccepted = true;
			if (!string.IsNullOrWhiteSpace(MessagesWordFilter) && e.Item is MatchModel)
			{
				isMessagesWordAccepted = false;
				string regex;
				if (MessagesWholeWordsFilter)
					regex = "\\b" + MessagesWordFilter.Replace(" ", "\\b|\\b") + "\\b";
				else
					regex = MessagesWordFilter.Replace(' ', '|');

				try
				{
					isMessagesWordAccepted = ((MatchModel) e.Item).Messages.Any
						(x => Regex.Match(x.Message, regex, RegexOptions.IgnoreCase).Success);
				}
				catch (ArgumentException)
				{
					// TODO escape special symbols
				}
			}

			// Gender
			bool isGenderAccepted = true;
			if (GenderFilter != Gender.Both)
				isGenderAccepted = gender == GenderFilter;

			// Messaged or not
			bool isMessagedAccepted = true;
			if (e.Item is MatchModel)
			{
				if (MessagedFilter == MessagedFilter.Messaged)
					isMessagedAccepted = ((MatchModel) e.Item).Messages.Count > 0;
				else if (MessagedFilter == MessagedFilter.NotMessaged)
					isMessagedAccepted = ((MatchModel) e.Item).Messages.Count == 0;
			}

			// Only new matches
			bool isNewMatch = true;
			if (OnlyNewMatches && e.Item is MatchModel)
				isNewMatch = MainVM.NewMatchList.Contains((MatchModel) e.Item);


			e.Accepted = isNameAccepted && isDescriptionAccepted && isDescriptionWordsAccepted
				&& isMinAgeAccepted && isMaxAgeAccepted
				&& isMinMessagesAccepted && isMaxMessagesAccepted && isMessagesWordAccepted
				&& isGenderAccepted && isMessagedAccepted && isNewMatch;

		}

		internal void UpdateStatusBar()
		{
			// Based on witch tab is selected, shows appropiate filtered count
			if ((string) SelectedTab.Header == Properties.Resources.main_tab_matches)
			{
				SelectedCount = MainVM.MatchList.Count;
				FilteredCount = FilteredMatchListCount;
			}
			else if ((string) SelectedTab.Header == Properties.Resources.main_tab_unmatched_me)
			{
				if (MainVM.UnmatchedMeList == null)
					MainVM.UnmatchedMeList = SerializationHelper.DeserializeUnmatchedList();
				SelectedCount = MainVM.UnmatchedMeList.Count;
				FilteredCount = FilteredUnmatchedMeListCount;
			}
			else if ((string) SelectedTab.Header == Properties.Resources.main_tab_unmatched_by_me)
			{
				if (MainVM.UnmatchedByMeList == null)
					MainVM.UnmatchedByMeList = SerializationHelper.DeserializeUnmatchedByMeList();
				SelectedCount = MainVM.UnmatchedByMeList.Count;
				FilteredCount = FilteredUnmatchedByMeListCount;
			}
			else if ((string) SelectedTab.Header == Properties.Resources.main_tab_recs_pending)
			{
				if (MainVM.RecommendationsPendingList == null)
					MainVM.RecommendationsPendingList = SerializationHelper.DeserializeRecPendingList();
				SelectedCount = MainVM.RecommendationsPendingList.Count;
				FilteredCount = FilteredRecommendationsPendingListCount;
			}
			else if ((string) SelectedTab.Header == Properties.Resources.main_tab_recs_passed)
			{
				if (MainVM.RecommendationsPassedList == null)
					MainVM.RecommendationsPassedList = SerializationHelper.DeserializeRecPassedList();
				SelectedCount = MainVM.RecommendationsPassedList.Count;
				FilteredCount = FilteredRecommendationsPassedListCount;
			}
		}

		
	}

	public enum SortingOptionsEnum
	{
		[Description("Activity Ascending")]
		ActivityAsc,
		[Description("Activity Descending")]
		ActivityDesc,
		[Description("Name Ascending")]
		NameAsc,
		[Description("Name Descending")]
		NameDesc,
		[Description("Match date Ascending")]
		MatchedAsc,
		[Description("Match date Descending")]
		MatchedDesc
	}
}