using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
			get { return (ISerializableItem)GetValue(SerializbleItemProperty); }
			set { SetValue(SerializbleItemProperty, value); }
		}

		public static readonly DependencyProperty SerializbleItemProperty =
			DependencyProperty.Register("SerializbleItem", typeof(ISerializableItem),
				typeof(PhotoScrollerView), new PropertyMetadata(null));

		public string MyDirPath
		{
			get { return (string)GetValue(DirPathProperty); }
			set { SetValue(DirPathProperty, value); }
		}

		public static readonly DependencyProperty DirPathProperty =
			DependencyProperty.Register("MyDirPath", typeof(string),
				typeof(PhotoScrollerView), new PropertyMetadata(null));
		
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

			string src = SerializationHelper.WorkingDir + MyDirPath;

			if (SerializbleItem != null)
				src += SerializbleItem + "\\";

			if (img.DataContext is InstagramPhoto)
			{
				string name = (img.DataContext as InstagramPhoto).Link;
				name = name.Remove(name.Length - 2);
				name = name.Substring(name.LastIndexOf("/") + 1) + ".jpg";
				src += SerializationHelper.IG_PHOTOS + name;
			}
			else
			{
				src += SerializationHelper.PHOTOS + (img.DataContext as PhotoModel).Id + ".jpg";
			}

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

		private void MenuItem_Copy(object sender, RoutedEventArgs e)
		{
			BitmapImage b = new BitmapImage();

			string src = SerializationHelper.WorkingDir + MyDirPath;

			if (SerializbleItem != null)
				src += SerializbleItem + "\\";

			if (photoList.SelectedItem is InstagramPhoto)
			{
				string name = (photoList.SelectedItem as InstagramPhoto).Link;
				name = name.Remove(name.Length - 2);
				name = name.Substring(name.LastIndexOf("/") + 1) + ".jpg";
				src += SerializationHelper.IG_PHOTOS + name;
			}
			else
			{
				src += SerializationHelper.PHOTOS + (photoList.SelectedItem as PhotoModel).Id + ".jpg";
			}
			
			if (File.Exists(src))
			{
				try
				{
					b.BeginInit();
					b.UriSource = new Uri(src);
					b.EndInit();
				}
				catch
				{ }
			}

			Clipboard.SetImage(b);
		}

		private void MenuItem_OpenFolder(object sender, RoutedEventArgs e)
		{
			string src = SerializationHelper.WorkingDir + MyDirPath;

			if (SerializbleItem != null)
				src += SerializbleItem + "\\";
			

			if (DataContext is ObservableCollection<PhotoModel>)
				src += SerializationHelper.PHOTOS;
			else
				src += SerializationHelper.IG_PHOTOS;

			Process.Start(src);
		}
	}
}
