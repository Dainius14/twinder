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
		}

		private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://instagram.com/" + (sender as TextBlock).Text);
		}
	}
}
