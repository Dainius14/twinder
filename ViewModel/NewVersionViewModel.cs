using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;
using Twinder.View;

namespace Twinder.ViewModel
{
	public class NewVersionViewModel : ViewModelBase
	{
		public string UrlToOpen { get; set; }

		public RelayCommand<NewVersionView> OpenPageCommand { get; private set; }

		public NewVersionViewModel()
		{
			OpenPageCommand = new RelayCommand<NewVersionView>((param) =>
				{
					param.Close();
					Process.Start(UrlToOpen);
				});
		}
	}
}
