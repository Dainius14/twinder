using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
			viewModel.LoadingStateChange += SwitchLoadingIndicators;
			viewModel.SetRecommendations(recList);
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

		private void SwitchLoadingIndicators(object sender, RecsLoadingStateEventArgs e)
		{
			if (e.RecsStatus == RecsStatus.Getting)
			{
				gettingRecs_StatusBarItem.Visibility = Visibility.Visible;
				gettingRecs_TextBlock.Text = Properties.Resources.tinder_recs_getting_recs;
			}
			else if (e.RecsStatus == RecsStatus.Exhausted)
			{
				gettingRecs_StatusBarItem.Visibility = Visibility.Visible;
				gettingRecs_TextBlock.Text = Properties.Resources.tinder_recs_exchausted;
			}
			else if (e.RecsStatus == RecsStatus.Okay)
			{
				gettingRecs_StatusBarItem.Visibility = Visibility.Collapsed;
				recCount_StatusBarItem.Visibility = Visibility.Visible;
			}
		}
	}
}
