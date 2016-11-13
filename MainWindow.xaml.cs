using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Twinder.Models.Updates;
using Twinder.ViewModel;
using System;
using Twinder.View;
using System.Collections.ObjectModel;
using Twinder.Model;
using Twinder.Models;
using System.Windows.Interop;
using Twinder.Helpers;

namespace Twinder
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Closing += (s, e) => ViewModelLocator.Cleanup();
			Messenger.Default.Register<MatchModel>(this, MessageType.NewChatWindow, CreateChatWindow);
			Messenger.Default.Register<MatchModel>(this, MessageType.ShowMatchProfile, CreateMatchProfileView);
			Messenger.Default.Register<string>(this, MessageType.ShowRecommendations, CreateRecommendationsWindow);
			Messenger.Default.Register<string>(this, MessageType.ShowSetLocationWindow, CreateSetLocationWindow);
		}

		private void CreateSetLocationWindow(string obj)
		{
			var locationWindow = new SetLocationView();
			locationWindow.Owner = this;
			locationWindow.ShowDialog();
		}

		private void CreateRecommendationsWindow(string str)
		{
			var recsWindow = new RecommendationsView();
			recsWindow.Owner = this;
			recsWindow.Show();
		}

		private void CreateChatWindow(MatchModel match)
		{
			var chatWindow = new ChatView(match);
			chatWindow.Owner = this;
			chatWindow.Show();
		}

		private void CreateMatchProfileView(MatchModel match)
		{
			var matchProfileWindow = new MatchProfileView(match);
			matchProfileWindow.Owner = this;
			matchProfileWindow.Show();
		}

		/// <summary>
		/// Handles double click on match list, which opens a new chat
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void matchList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			MainViewModel viewModel = DataContext as MainViewModel;
			ListViewItem item = sender as ListViewItem;
			MatchModel match = item.Content as MatchModel;

			// Workaround for losing focus
			Action newWindow = () => viewModel.OpenChatCommand.Execute(match);
			Dispatcher.BeginInvoke(newWindow);
		}

		/// <summary>
		/// Explicit shutdown operation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		/// <summary>
		/// Starts loading content, after the window is rendered
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Window_ContentRendered(object sender, EventArgs e)
		{
			var viewModel = DataContext as MainViewModel;
			authText.Text = Properties.Resources.auth_connecting;
			if (await viewModel.Authenticate())
			{
				authText.Text = Properties.Resources.auth_getting_matches;
				if (await viewModel.GetMatches())
				{
					authText.Text = Properties.Resources.auth_okay;
					matchList.SelectedIndex = 0;
				}
				else
				{
					authText.Text = Properties.Resources.auth_get_matches_error;
				}
			}
			else
			{
				authText.Text = Properties.Resources.auth_connect_error;
			}
			authProgressBar.Visibility = Visibility.Collapsed;


		}

	}
}