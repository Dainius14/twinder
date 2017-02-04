using System.Collections.ObjectModel;
using Twinder.Model;
using Twinder.ViewModel;

namespace Twinder.SampleData
{
	public class SampleAccountSwitchVM : AccountSwitchViewModel
	{

		private ObservableCollection<AccountModel> _accountList;
		public new ObservableCollection<AccountModel> AccountList
		{
			get { return _accountList; }
			set { Set(ref _accountList, value); }
		}


		public SampleAccountSwitchVM()
		{
			AccountList = new ObservableCollection<AccountModel>()
			{
				new AccountModel()
				{
					Name = "Morpheus",
					MatchCount = 69,
					Photo = @"D:\Documents\GitHub\twinder\SampleData\morpheus.jpg"
				}
			};
		}
	}
}
