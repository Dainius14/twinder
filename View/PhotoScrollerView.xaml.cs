using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.Model.Photos;

namespace Twinder.View
{
	public partial class PhotoScrollerView : UserControl
	{


		public ISerializableItem SerializbleItem
		{
			get { return (ISerializableItem) GetValue(SerializbleItemProperty); }
			set { SetValue(SerializbleItemProperty, value); }
		}

		public static readonly DependencyProperty SerializbleItemProperty =
			DependencyProperty.Register("SerializbleItem", typeof(ISerializableItem),
				typeof(PhotoScrollerView), new PropertyMetadata(null));



		public string DirPath
		{
			get { return (string) GetValue(DirPathProperty); }
			set { SetValue(DirPathProperty, value); }
		}

		public static readonly DependencyProperty DirPathProperty =
			DependencyProperty.Register("DirPath", typeof(string),
				typeof(PhotoScrollerView), new PropertyMetadata(string.Empty));

		

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
				ScrollBar.LineRightCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			if (e.Delta > 0)
			{
				ScrollBar.LineLeftCommand.Execute(null, e.OriginalSource as IInputElement);
				ScrollBar.LineLeftCommand.Execute(null, e.OriginalSource as IInputElement);
				ScrollBar.LineLeftCommand.Execute(null, e.OriginalSource as IInputElement);
			}
			e.Handled = true;
		}

		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage b = new BitmapImage();
			Image img = sender as Image;
			
			var src = SerializationHelper.WorkingDir + DirPath
				+ SerializbleItem + "\\" + SerializationHelper.PHOTOS + (img.DataContext as PhotoModel).Id + ".jpg";

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
