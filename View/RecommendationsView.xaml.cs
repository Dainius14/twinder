using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class RecommendationsView : Window
	{
		public RecommendationsView()
		{
			InitializeComponent();
		}

		public RecommendationsView(ObservableCollection<RecModel> recList)
		{
			InitializeComponent();
			var viewModel = DataContext as RecommendationsViewModel;
			//viewModel.LoadingStateChange += SwitchLoadingIndicators;
			viewModel.SetRecommendations(recList);
			viewModel.RecsView = this;

			Messenger.Default.Register<SerializationPacket>(this, ShowDownloadDialog);

		}

		private void ShowDownloadDialog(SerializationPacket packet)
		{
			var downloadDialog = new DownloadDataView(packet);
			downloadDialog.Owner = this;
			downloadDialog.ShowDialog();
		}


		/// <summary>
		/// Scrolls the listview
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta < 0)
			{
				// Scrolls a little bit more than one line
				ScrollBar.LineRightCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			if (e.Delta > 0)
			{
				ScrollBar.LineLeftCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			e.Handled = true;
		}
	}
}
