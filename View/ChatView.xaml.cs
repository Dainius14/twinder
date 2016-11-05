using System.Windows;
using Twinder.Models.Updates;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class ChatView : Window
	{
		public ChatView()
		{
			InitializeComponent();
		}

		public ChatView(MatchModel match)
		{
			InitializeComponent();
			var viewModel = DataContext as ChatViewModel;
			viewModel.Match = match;
		}
	}
}
