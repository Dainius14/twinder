using System;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
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
			chatScrollViewer.ScrollToEnd();

			viewModel.NewChatMessageReceived += FlashWindow;
			Loaded += (sender, e) => message_TextBox.Focus();

		}

		/// <summary>
		/// Scrolls to bottom when a new message is sent
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chatBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			chatScrollViewer.ScrollToEnd();
		}

		/// <summary>
		/// Starts flashing window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FlashWindow(object sender, EventArgs e)
		{
			// If is active, flashes only once
			if (IsActive)
				WindowFlasher.Flash(this, 1);
			else
				WindowFlasher.Start(this);
		}
		
		/// <summary>
		/// Stops flashing window when it is activated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Activated(object sender, EventArgs e)
		{
			WindowFlasher.Stop(this);
		}
	}
}
