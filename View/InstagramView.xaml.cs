using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for InstagramView.xaml
	/// </summary>
	public partial class InstagramView : Window
	{
		public InstagramView()
		{
			InitializeComponent();

			// Close window with ESC
			PreviewKeyDown += (object sender, KeyEventArgs e) =>
			{
				if (e.Key == Key.Escape)
					Close();
			};
		}

		private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://instagram.com/" + (sender as TextBlock).Text);
		}
	}
}
