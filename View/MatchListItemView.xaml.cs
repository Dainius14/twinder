using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Twinder.Helpers;
using Twinder.Model;

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

		}

		private void CreateMatchProfileView(MatchModel match)
		{
			MatchProfileView window = new MatchProfileView(match);
			window.Show();
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Binding myBinding = new Binding();

			myBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListView), 1);
		}

		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage b = new BitmapImage();
			Image img = sender as Image;

			var match = img.DataContext as MatchModel;
			if (match.Person.Photos.Count == 0)
				return;
			var src = SerializationHelper.WorkingDir + SerializationHelper.DIR_MATCHES
				+ match + "\\" + SerializationHelper.PHOTOS + match.Person.Photos[0].FileName;

			if (File.Exists(src))
			{
				try
				{
					b.BeginInit();
					b.CacheOption = BitmapCacheOption.OnLoad;
					b.UriSource = new Uri(src);
					b.EndInit();
					img.Source = b;
				}
				catch
				{
				}
			}

		}
	}
}
