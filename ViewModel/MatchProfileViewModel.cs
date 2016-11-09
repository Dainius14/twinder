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
			}
		}
		
	}
}
