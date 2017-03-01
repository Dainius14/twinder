using System.Windows;
using Twinder.ViewModel;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for NewVersionView.xaml
	/// </summary>
	public partial class NewVersionView : Window
	{
		public NewVersionView(string urlToOpen)
		{
			InitializeComponent();
			var viewModel = DataContext as NewVersionViewModel;
			viewModel.UrlToOpen = urlToOpen;
		}
	}
}
