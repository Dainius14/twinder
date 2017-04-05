using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Windows;
using Twinder.Helpers;
using Twinder.ViewModel;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for DownloadDataView.xaml
	/// </summary>
	public partial class DownloadDataView : Window
	{
		public DownloadDataView(SerializationPacket packet)
		{
			InitializeComponent();

			var viewModel = DataContext as DownloadDataViewModel;
			viewModel.Packet = packet;
			// Fuck MVVM pattern for simplicity
			viewModel.MyView = this;
			viewModel.Worker.Disposed += (object sender, EventArgs e) => Close();
			viewModel.Worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
			{
				if (packet.RecList != null && packet.RecList.Count != 0)
					Messenger.Default.Send(packet.RecList, MessengerToken.OpenRecommendations);
				Close();
			};
			viewModel.StartDownload();
		}
	}
}
