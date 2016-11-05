using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Twinder.Models.Updates;
using Twinder.ViewModel;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for MatchListItemView.xaml
	/// </summary>
	public partial class MatchListItemView : UserControl
	{
		public MatchListItemView()
		{
			InitializeComponent();
			//Messenger.Default.Register<MatchModel>(this, MessageType.ShowMatchProfile, CreateMatchProfileView);
		}

		private void CreateMatchProfileView(MatchModel match)
		{
			MatchProfileView window = new MatchProfileView(match);
			window.Show();
		}
		
	}
}
