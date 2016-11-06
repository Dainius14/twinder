using GalaSoft.MvvmLight;
using System;
using Twinder.Models.Updates;

namespace Twinder.ViewModel
{
	public class MatchProfileViewModel : ViewModelBase
	{
		private MatchModel _match;
		public MatchModel Match
		{
			get { return _match; }
			set
			{
				Set(ref _match, value);
				WindowTitle = String.Format($"{Match.Person.Name} Profile | {Properties.Resources.app_title}");
				RaisePropertyChanged("WindowTitle");
			}
		}


		private string _windowTitle;
		public string WindowTitle
		{
			get { return _windowTitle; }
			set { Set(ref _windowTitle, value); }
		}
	}
}
