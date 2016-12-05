using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
			viewModel.Worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => Close();
			viewModel.StartDownload();
		}
	}
}
