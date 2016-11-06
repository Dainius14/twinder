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
			this.Focus();
			var viewModel = DataContext as ChatViewModel;
			viewModel.Match = match;
			chatScrollViewer.ScrollToEnd();
		}

		private void chatBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			chatScrollViewer.ScrollToEnd();
		}
	}
}
