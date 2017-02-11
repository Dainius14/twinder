using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using Twinder.Model;

namespace Twinder.ViewModel
{
	public class MatchListFilterViewModel : ViewModelBase
	{
		/// <summary>
		/// Holds reference to main view model to access needed things
		/// </summary>
		public MainViewModel MainVM { get; private set; }

		private int _filteredMatchListCount;
		public int FilteredMatchListCount
		{
			get { return _filteredMatchListCount; }
			set { Set(ref _filteredMatchListCount, value); }
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
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private int? _minAgeFilter;
		public int? MinAgeFilter
		{
			get { return _minAgeFilter; }
			set
			{
				Set(ref _minAgeFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private int? _maxAgeFilter;
		public int? MaxAgeFilter
		{
			get { return _maxAgeFilter; }
			set
			{
				Set(ref _maxAgeFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}


		private DescriptionFilter _descriptionFilter;
		public DescriptionFilter DescriptionFilter
		{
			get { return _descriptionFilter; }
			set
			{
				Set(ref _descriptionFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private string _descriptionWordFilter;
		public string DescriptionWordFilter
		{
			get { return _descriptionWordFilter; }
			set
			{
				Set(ref _descriptionWordFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private bool _descriptionWholeWordsFilter = false;
		public bool DescriptionWholeWordsFilter
		{
			get { return _descriptionWholeWordsFilter; }
			set
			{
				Set(ref _descriptionWholeWordsFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}


		private string _messagesWordFilter;
		public string MessagesWordFilter
		{
			get { return _messagesWordFilter; }
			set
			{
				Set(ref _messagesWordFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private bool _messagesWholeWordsFilter = false;
		public bool MessagesWholeWordsFilter
		{
			get { return _messagesWholeWordsFilter; }
			set
			{
				Set(ref _messagesWholeWordsFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}


		private int? _minMessagesFilter;
		public int? MinMessagesFilter
		{
			get { return _minMessagesFilter; }
			set
			{
				Set(ref _minMessagesFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private int? _maxMessagesFilter;
		public int? MaxMessagesFilter
		{
			get { return _maxMessagesFilter; }
			set
			{
				Set(ref _maxMessagesFilter, value);
				MainVM.MatchListCvs.View.Refresh();
			}
		}

		private Gender _genderFilter = Gender.Both;
		public Gender GenderFilter
		{
			get { return _genderFilter; }
			set
			{
				Set(ref _genderFilter, value);
				MainVM.MatchListCvs.View.Refresh();
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

			if (MainVM.MatchListCvs != null)
				using (MainVM.MatchListCvs.DeferRefresh())
				{
					MainVM.MatchListCvs.SortDescriptions.Clear();
					MainVM.MatchListCvs.SortDescriptions.Add(sortDescription);
				}
		}

		/// <summary>
		/// Applies filtering on given options
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void MatchList_ApplyFilter(object sender, FilterEventArgs e)
		{
			var match = (MatchModel) e.Item;

			// Name
			bool isNameAccepted;
			if (string.IsNullOrWhiteSpace(NameFilter))
				isNameAccepted = true;
			else
				isNameAccepted = match.Person.Name.ToLower().StartsWith(NameFilter.ToLower());

			// Description is null or not
			bool isDescriptionAccepted;
			if (DescriptionFilter == DescriptionFilter.Both)
				isDescriptionAccepted = true;
			else if (DescriptionFilter == DescriptionFilter.WithDescription)
				isDescriptionAccepted = !string.IsNullOrWhiteSpace(match.Person.Bio);
			else
				isDescriptionAccepted = string.IsNullOrWhiteSpace(match.Person.Bio);

			// Description words
			bool isDescriptionWordsAccepted;
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
					isDescriptionWordsAccepted = Regex.Match(match.Person.Bio, regex, RegexOptions.IgnoreCase).Success;
				}
				catch (ArgumentException)
				{
					// TODO escape special symbols
				}


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

				try
				{
					isMessagesWordAccepted = match.Messages.Any
						(x => Regex.Match(x.Message, regex, RegexOptions.IgnoreCase).Success);
				}
				catch (ArgumentException)
				{
					// TODO escape special symbols
				}
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