using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twinder.Model.Photos;

namespace Twinder.View
{
	public partial class PhotoScrollerView : UserControl
	{
		public PhotoScrollerView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles image scrolling
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
		
		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage b = new BitmapImage();
			Image img = sender as Image;
			try
			{
				b.BeginInit();
				b.CacheOption = BitmapCacheOption.OnLoad;
				b.UriSource = new Uri((img.DataContext as PhotoModel).ProcessedFiles[0].LocalUrl);
				b.EndInit();

				img.Source = b;
			}
			catch (Exception)
			{
			}
		}
	}
}
