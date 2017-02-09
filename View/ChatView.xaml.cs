using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.Model.Photos;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class ChatView : Window
	{
		private MatchModel _match;
		public ChatView()
		{
			InitializeComponent();
		}

		public ChatView(MatchModel match)
		{
			InitializeComponent();
			var viewModel = DataContext as ChatViewModel;
			_match = match;
			viewModel.Match = match;

			int scrollTo = Chat_ListView.Items.Count - 1;
			if (scrollTo > 0)
				Chat_ListView.ScrollIntoView(Chat_ListView.Items[scrollTo]);


			viewModel.NewChatMessageReceived += FlashWindow;
			Loaded += (sender, e) => SendMessage_TextBox.Focus();
			

		}

		/// <summary>
		/// Scrolls to bottom when a new message is sent
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chatBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			int scrollTo = Chat_ListView.Items.Count - 1;
			if (scrollTo > 0)
				Chat_ListView.ScrollIntoView(Chat_ListView.Items[scrollTo]);
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
		

		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage b = new BitmapImage();
			Image img = sender as Image;

			var src = SerializationHelper.WorkingDir + SerializationHelper.DIR_MATCHES
				+ _match + "\\" + SerializationHelper.PHOTOS + _match.Person.Photos[0].Id + ".jpg";

			if (File.Exists(src))
			{
				b.BeginInit();
				b.CacheOption = BitmapCacheOption.OnLoad;
				b.UriSource = new Uri(src);
				b.EndInit();
			}

			img.Source = b;
		}

		/// <summary>
		/// Opens profile on header click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HeaderBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var matchProfileWindow = new MatchProfileView(_match);
			matchProfileWindow.Show();
		}
	}
}
