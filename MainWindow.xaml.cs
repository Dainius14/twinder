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
using System.ComponentModel;
using System.Windows.Data;

namespace Twinder
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Closing += (s, e) => ViewModelLocator.Cleanup();
			Messenger.Default.Register<MatchModel>(this, MessengerToken.NewChatWindow, CreateChatWindow);
			Messenger.Default.Register<MatchModel>(this, MessengerToken.ShowMatchProfile, CreateMatchProfileView);
			Messenger.Default.Register<UserModel>(this, MessengerToken.OpenMyProfile, CreateMyProfileWindow);
			Messenger.Default.Register<ObservableCollection<RecModel>>(this, MessengerToken.OpenRecommendations, OpenRecsWindow);
			Messenger.Default.Register<string>(this, MessengerToken.ShowSetLocationWindow, CreateSetLocationWindow);
			Messenger.Default.Register<string>(this, MessengerToken.ShowLoginDialog, CreateLoginWindow);
			Messenger.Default.Register<string>(this, MessengerToken.RefreshMatchList, RefreshMatchList);

			var myViewModel = DataContext as MainViewModel;
			myViewModel.MyView = this;
			myViewModel.ConnectionStatusChanged += UpdateStatusBar;
			

			Messenger.Default.Register<string>(this, MessengerToken.SortMatchList, SortMatchList);
		}

		private void SortMatchList(string obj)
		{
			string propertyName = nameof(MatchModel.LastActivityDate);

			using (matchList.Items.DeferRefresh())
			{
				ListSortDirection direction = ListSortDirection.Descending;
				matchList.Items.SortDescriptions.Add(new SortDescription(propertyName, direction));
			}
		}

		private void RefreshMatchList(string obj)
		{
			var matchListProperty = CollectionViewSource.GetDefaultView(matchList.ItemsSource);
			matchListProperty.Refresh();
		}

		private void CreateMyProfileWindow(UserModel user)
		{
			var myProfileWindow = new UserProfileView(user);
			myProfileWindow.Show();
		}

		private void CreateLoginWindow(string obj)
		{
			var loginWindow = new FbLoginView();
			loginWindow.Owner = this;
			loginWindow.ShowDialog();
		}
		

		private void CreateSetLocationWindow(string obj)
		{
			var locationWindow = new SetLocationView();
			locationWindow.ShowDialog();
		}

		private void OpenRecsWindow(ObservableCollection<RecModel> recList)
		{
			var recsWindow = new RecommendationsView(recList);
			recsWindow.Show();
		}

		private void CreateChatWindow(MatchModel match)
		{
			var chatWindow = new ChatView(match);
			chatWindow.Show();
		}

		private void CreateMatchProfileView(MatchModel match)
		{
			var matchProfileWindow = new MatchProfileView(match);
			matchProfileWindow.Show();
		}

		/// <summary>
		/// Handles double click on match list, which opens a new chat
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void matchList_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var viewModel = DataContext as MainViewModel;
			var item = sender as ListViewItem;
			var match = item.Content as MatchModel;

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
		/// Updates status bar based on current connection status
		/// TODO could be managed better I guess, should look into this
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateStatusBar(object sender, ConnectionStatusEventArgs e)
		{
			// Is authenticating
			if (e.AuthStatus == AuthStatus.Connecting)
			{
				connection_ProgressBar.Visibility = Visibility.Visible;
				connection_TextBlock.Text = Properties.Resources.tinder_auth_connecting;
			}
			// Authentication error
			else if (e.AuthStatus == AuthStatus.Error)
			{
				connection_ProgressBar.Visibility = Visibility.Collapsed;
				connection_TextBlock.Text = Properties.Resources.tinder_auth_error;
			}
			// Authentication ok
			else
			{
				// Getting both recommendations and matches
				if (e.MatchesStatus == MatchesStatus.Getting && e.RecsStatus == RecsStatus.Getting)
					connection_TextBlock.Text = Properties.Resources.tinder_getting_recs_matches;
				
				// Getting matches
				else if (e.MatchesStatus == MatchesStatus.Getting)
					connection_TextBlock.Text = Properties.Resources.tinder_update_getting_matches;

				// Getting recommendations
				else if (e.RecsStatus == RecsStatus.Getting)
					connection_TextBlock.Text = Properties.Resources.tinder_recs_getting_recs;


				// Matches
				if (e.MatchesStatus == MatchesStatus.Okay)
					matchCount_StatusBarItem.Visibility = Visibility.Visible;
				else if (e.MatchesStatus == MatchesStatus.Error)
				{
					matchCount_StatusBarItem.Visibility = Visibility.Visible;
					matchCountError_TextBlock.Visibility = Visibility.Visible;
					matchCountOk_TextBlock.Visibility = Visibility.Collapsed;
				}

				// Recommendations
				if (e.RecsStatus == RecsStatus.Okay)
					recCount_StatusBarItem.Visibility = Visibility.Visible;
				else if (e.RecsStatus == RecsStatus.Exhausted)
				{
					recCount_StatusBarItem.Visibility = Visibility.Visible;
					recCountExhausted_TextBlock.Visibility = Visibility.Visible;
					recCountOk_TextBlock.Visibility = Visibility.Collapsed;
				}
				else if (e.RecsStatus == RecsStatus.Error)
				{
					recCount_StatusBarItem.Visibility = Visibility.Visible;
					recCountError_TextBlock.Visibility = Visibility.Visible;
					recCountOk_TextBlock.Visibility = Visibility.Collapsed;
				}

				// Everything connected
				if (e.MatchesStatus != MatchesStatus.Getting && e.MatchesStatus != MatchesStatus.Waiting
					&& e.RecsStatus != RecsStatus.Getting && e.RecsStatus != RecsStatus.Waiting)
				{
					connection_ProgressBar.Visibility = Visibility.Collapsed;
					connection_TextBlock.Text = Properties.Resources.tinder_auth_okay;
				}
			}
		}
		
	}
}