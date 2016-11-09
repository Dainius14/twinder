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

		private async void Window_ContentRendered(object sender, System.EventArgs e)
		{
			var viewModel = DataContext as RecommendationsViewModel;
			authText.Text = Properties.Resources.auth_getting_recs;
			if (await viewModel.GetRecommendations())
			{
				auth_get_recs.Visibility = Visibility.Collapsed;
				auth_sep.Visibility = Visibility.Collapsed;
			}
			else
				authText.Text = Properties.Resources.auth_recs_exchausted;
		}
	}
}
