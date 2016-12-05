using GalaSoft.MvvmLight;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;

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
				//GetFullInfo();
			}
		}

		private async void GetFullInfo()
		{
			try
			{
				await TinderHelper.GetFullMatchData(Match.Person.Id);
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
			}
		}
	}
}
