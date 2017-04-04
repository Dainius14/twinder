using System;
using System.Collections.ObjectModel;
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
		public string DirPath
		{
			get { return (string) GetValue(DirPathProperty); }
			set { SetValue(DirPathProperty, value); }
		}

		public static readonly DependencyProperty DirPathProperty =
			DependencyProperty.Register("DirPath", typeof(string),
				typeof(PhotoScrollerView), new PropertyMetadata(string.Empty));




		public string MatchName
		{
			get { return (string) GetValue(MatchNameProperty); }
			set { SetValue(MatchNameProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MatchNameProperty =
			DependencyProperty.Register("MatchName", typeof(string), typeof(MatchListItemView), null);



		public int MatchAge
		{
			get { return (int) GetValue(MatchAgeProperty); }
			set { SetValue(MatchAgeProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MatchAge.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MatchAgeProperty =
			DependencyProperty.Register("MatchAge", typeof(int), typeof(MatchListItemView), null);



		public string Bio
		{
			get { return (string) GetValue(BioProperty); }
			set { SetValue(BioProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Bio.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty BioProperty =
			DependencyProperty.Register("Bio", typeof(string), typeof(MatchListItemView), null);



		public DateTime LastSeen
		{
			get { return (DateTime) GetValue(LastSeenProperty); }
			set { SetValue(LastSeenProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LastSeen.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LastSeenProperty =
			DependencyProperty.Register("LastSeen", typeof(DateTime), typeof(MatchListItemView), null);


		public DateTime MatchedOn
		{
			get { return (DateTime) GetValue(MatchedOnProperty); }
			set { SetValue(MatchedOnProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MatchedOn.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MatchedOnProperty =
			DependencyProperty.Register("MatchedOn", typeof(DateTime), typeof(MatchListItemView), null);


		public ObservableCollection<MessageModel> Messages
		{
			get { return (ObservableCollection<MessageModel>) GetValue(MessagesProperty); }
			set { SetValue(MessagesProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessagesProperty =
			DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageModel>), typeof(MatchListItemView), null);




		public bool IsMatchModel
		{
			get { return (bool) GetValue(IsMatchModelProperty); }
			set { SetValue(IsMatchModelProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsMatchModel.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsMatchModelProperty =
			DependencyProperty.Register("IsMatchModel", typeof(bool), typeof(MatchListItemView), null);




		public MatchListItemView()
		{
			InitializeComponent();
			DataContextChanged += MatchListItemView_DataContextChanged;

		}

		private void MatchListItemView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext is MatchModel)
			{
				IsMatchModel = true;
				var match = (MatchModel) DataContext;
				MatchName = match.Person.Name;
				MatchAge = match.Person.Age;
				Bio = match.Person.Bio;
				MatchedOn = match.CreatedDate;
				LastSeen = match.LastActivityDate;
				Messages = match.Messages;
			}
			else if (DataContext is RecModel)
			{
				IsMatchModel = false;
				var rec = (RecModel) DataContext;
				MatchName = rec.Name;
				MatchAge = rec.Age;
				Bio = rec.Bio;
			}
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

			string src = string.Empty;
			if (img.DataContext is MatchModel)
			{
				var match = img.DataContext as MatchModel;
				if (match.Person.Photos.Count == 0)
					return;
				src = SerializationHelper.WorkingDir + DirPath + "\\"
					+ match + "\\" + SerializationHelper.PHOTOS + match.Person.Photos[0].FileName;
			}
			else if (img.DataContext is RecModel)
			{
				var rec = img.DataContext as RecModel;
				if (rec.Photos.Count == 0)
					return;
				src = SerializationHelper.WorkingDir + DirPath + "\\"
					+ rec + "\\" + SerializationHelper.PHOTOS + rec.Photos[0].Id + ".jpg";
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
	}
}
