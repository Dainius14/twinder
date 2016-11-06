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
			Messenger.Default.Register<RecsResultsModel>(this, MessageType.ShowRecommendations, CreateRecommendationsWindow);
			Messenger.Default.Register<NotificationMessageAction<bool>>(this, r => 
			{
				var dialog = new FbLoginView();
				var returned = dialog.ShowDialog();
				r.Execute((bool) returned);

			});
		}

		private void ShowLoginDialog(NotificationMessageAction<bool> obj)
		{
			var dialog = new FbLoginView();
			var returned = dialog.ShowDialog();
			obj.Execute((bool) returned);


		}
		
		private void CreateRecommendationsWindow(RecsResultsModel results)
		{
			var recsWindow = new RecommendationsView(results);
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
		/// Scrolls match list by one entry only
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta < 0)
			{
				ScrollBar.LineDownCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			if (e.Delta > 0)
			{
				ScrollBar.LineUpCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			e.Handled = true;
		}

		/// <summary>
		/// Handles double click on match list, which opens a new chat
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			MainViewModel viewModel = DataContext as MainViewModel;
			ListViewItem item = sender as ListViewItem;
			MatchModel match = item.Content as MatchModel;

			// Workaround for losing focus
			Action newWindow = () => viewModel.OpenChatCommand.Execute(match);
			Dispatcher.BeginInvoke(newWindow);
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

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