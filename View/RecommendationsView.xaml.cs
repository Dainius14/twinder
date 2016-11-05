using System.Windows;
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

		public RecommendationsView(RecsResultsModel results)
		{
			InitializeComponent();
			var viewModel = DataContext as RecommendationsViewModel;
			viewModel.Recommendations = results.Recommendations;
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
				ScrollBar.LineRightCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			if (e.Delta > 0)
			{
				ScrollBar.LineLeftCommand.Execute(null, e.OriginalSource as IInputElement);
				ScrollBar.LineLeftCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			e.Handled = true;
		}
	}
}
