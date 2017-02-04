using GalaSoft.MvvmLight;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.ViewModel;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for AccountSwitchView.xaml
	/// </summary>
	public partial class AccountSwitchView : Window
	{
		public AccountSwitchView()
		{
			InitializeComponent();
		}

		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			BitmapImage b = new BitmapImage();
			Image img = sender as Image;
			try
			{
				var account = img.DataContext as AccountModel;
				string src;
				if (!ViewModelBase.IsInDesignModeStatic)
				{
					src = Properties.Settings.Default.AppDataFolder + account + "\\"
						+ SerializationHelper.DIR_USER + SerializationHelper.PHOTOS + account.Photo;
				}
				else
				{
					src = account.Photo;
				}

				if (File.Exists(src))
				{
					b.BeginInit();
					b.CacheOption = BitmapCacheOption.OnLoad;
					b.UriSource = new Uri(src);
					b.EndInit();
				}

				img.Source = b;
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Handles double click on list, which logs in selected user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			(DataContext as AccountSwitchViewModel).OkCommand.Execute(this);
		}

	}
}
