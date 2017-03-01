using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Twinder.ViewModel;
using System;
using Twinder.View;
using System.Collections.ObjectModel;
using Twinder.Model;
using System.ComponentModel;
using System.Windows.Data;
using Twinder.Helpers;

namespace Twinder
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			var myViewModel = DataContext as MainViewModel;
			myViewModel.MyView = this;
			// Once loaded, starts setting up all the data

			this.Loaded += myViewModel.StartInitialize;
			this.Closing += (s, e) => ViewModelLocator.Cleanup();

			Messenger.Default.Register<MatchModel>(this, MessengerToken.NewChatWindow, CreateChatWindow);
			Messenger.Default.Register<MatchModel>(this, MessengerToken.ShowMatchProfile, CreateMatchProfileView);
			Messenger.Default.Register<UserModel>(this, MessengerToken.OpenMyProfile, CreateMyProfileWindow);
			Messenger.Default.Register<ObservableCollection<RecModel>>(this, MessengerToken.OpenRecommendations, OpenRecsWindow);
			Messenger.Default.Register<string>(this, MessengerToken.ShowSetLocationWindow, CreateSetLocationWindow);
			Messenger.Default.Register<string>(this, MessengerToken.ShowUpdateAvailableDialog, ShowUpdateAvailableDialog);
			Messenger.Default.Register<SerializationPacket>(this, MessengerToken.ShowSerializationDialog, ShowDownloadDialog);


		}

		private void ShowUpdateAvailableDialog(string obj)
		{
			var updateDialog = new NewVersionView(obj);
			updateDialog.Owner = this;
			updateDialog.ShowDialog();
		}

		private void ShowDownloadDialog(SerializationPacket packet)
		{
			var downloadDialog = new DownloadDataView(packet);
			downloadDialog.Owner = this;
			downloadDialog.ShowDialog();
		}
		
		private void CreateMyProfileWindow(UserModel user)
		{
			var myProfileWindow = new UserProfileView(user);
			myProfileWindow.Show();
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
		
	}
}