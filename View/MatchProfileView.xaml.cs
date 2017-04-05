using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Twinder.Model;
using Twinder.Model.UserRelated;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class MatchProfileView : Window
	{
		public string DirPath
		{
			get { return (string)GetValue(DirPathProperty); }
			set { SetValue(DirPathProperty, value); }
		}

		public static readonly DependencyProperty DirPathProperty =
			DependencyProperty.Register("DirPath", typeof(string),
				typeof(MatchProfileView), new PropertyMetadata(string.Empty));




		public string MatchName
		{
			get { return (string)GetValue(MatchNameProperty); }
			set { SetValue(MatchNameProperty, value); }
		}

		public static readonly DependencyProperty MatchNameProperty =
			DependencyProperty.Register("MatchName", typeof(string),
				typeof(MatchProfileView), new PropertyMetadata(string.Empty));



		public int MatchAge
		{
			get { return (int)GetValue(MatchAgeProperty); }
			set { SetValue(MatchAgeProperty, value); }
		}

		public static readonly DependencyProperty MatchAgeProperty =
			DependencyProperty.Register("MatchAge", typeof(int),
				typeof(MatchProfileView), new PropertyMetadata(1));



		public string Bio
		{
			get { return (string)GetValue(BioProperty); }
			set { SetValue(BioProperty, value); }
		}

		public static readonly DependencyProperty BioProperty =
			DependencyProperty.Register("Bio", typeof(string),
				typeof(MatchProfileView), new PropertyMetadata(string.Empty));



		public DateTime LastSeen
		{
			get { return (DateTime)GetValue(LastSeenProperty); }
			set { SetValue(LastSeenProperty, value); }
		}

		// Using a DependencyProperty as the backing store for LastSeen.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LastSeenProperty =
			DependencyProperty.Register("LastSeen", typeof(DateTime),
				typeof(MatchProfileView), null);


		public DateTime MatchedOn
		{
			get { return (DateTime)GetValue(MatchedOnProperty); }
			set { SetValue(MatchedOnProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MatchedOn.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MatchedOnProperty =
			DependencyProperty.Register("MatchedOn", typeof(DateTime), typeof(MatchProfileView), null);


		public ObservableCollection<MessageModel> Messages
		{
			get { return (ObservableCollection<MessageModel>)GetValue(MessagesProperty); }
			set { SetValue(MessagesProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Messages.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessagesProperty =
			DependencyProperty.Register("Messages", typeof(ObservableCollection<MessageModel>), typeof(MatchProfileView), null);




		public bool IsMatchModel
		{
			get { return (bool)GetValue(IsMatchModelProperty); }
			set { SetValue(IsMatchModelProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsMatchModel.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsMatchModelProperty =
			DependencyProperty.Register("IsMatchModel", typeof(bool),
				typeof(MatchProfileView), new PropertyMetadata(false));





		public int OurLikes
		{
			get { return (int)GetValue(OurLikesProperty); }
			set { SetValue(OurLikesProperty, value); }
		}

		// Using a DependencyProperty as the backing store for OurLikes.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OurLikesProperty =
			DependencyProperty.Register("OurLikes", typeof(int), typeof(MatchProfileView), new PropertyMetadata(0));



		public int OurFriends
		{
			get { return (int)GetValue(OurFriendsProperty); }
			set { SetValue(OurFriendsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for OurFriends.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OurFriendsProperty =
			DependencyProperty.Register("OurFriends", typeof(int), typeof(MatchProfileView), new PropertyMetadata(0));




		public ObservableCollection<SchoolModel> Schools
		{
			get { return (ObservableCollection<SchoolModel>)GetValue(SchoolProperty); }
			set { SetValue(SchoolProperty, value); }
		}

		// Using a DependencyProperty as the backing store for School.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SchoolProperty =
			DependencyProperty.Register("Schools", typeof(ObservableCollection<SchoolModel>), typeof(MatchProfileView), new PropertyMetadata(null));



		public ObservableCollection<JobModel> Jobs
		{
			get { return (ObservableCollection<JobModel>)GetValue(JobProperty); }
			set { SetValue(JobProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Job.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty JobProperty =
			DependencyProperty.Register("Jobs", typeof(ObservableCollection<JobModel>), typeof(MatchProfileView), new PropertyMetadata(null));



		public int TheDistance
		{
			get { return (int)GetValue(TheDistanceProperty); }
			set { SetValue(TheDistanceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TheDistance.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TheDistanceProperty =
			DependencyProperty.Register("TheDistance", typeof(int), typeof(MatchProfileView), new PropertyMetadata(0));



		private void MatchProfileView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (DataContext is MatchModel)
			{
				IsMatchModel = true;
				var match = (MatchModel)DataContext;
				MatchName = match.Person.Name;
				MatchAge = match.Person.Age;
				Bio = match.Person.Bio;
				MatchedOn = match.CreatedDate;
				LastSeen = match.LastActivityDate;
				Messages = match.Messages;
				OurLikes = match.CommonLikeCount;
				OurFriends = match.CommonFriendCount;
				Schools = match.Schools;
				Jobs = match.Jobs;
				TheDistance = match.DistanceMiles;
				PhotoScroller.DataContext = match.Person.Photos;

				if (match.Instagram != null)
					IGButton.IsEnabled = true;
			}
			else if (DataContext is RecModel)
			{
				IsMatchModel = false;
				var rec = (RecModel)DataContext;
				MatchName = rec.Name;
				MatchAge = rec.Age;
				Bio = rec.Bio;
				OurLikes = rec.CommonLikeCount;
				OurFriends = rec.CommonFriendCount;
				Schools = rec.Schools;
				Jobs = rec.Jobs;
				PhotoScroller.DataContext = rec.Photos;

				if (rec.Instagram != null)
					IGButton.IsEnabled = true;
			}
		}

		public MatchProfileView(ISerializableItem item, string dir)
		{
			InitializeComponent();
			//MatchProfileViewModel viewModel = DataContext as MatchProfileViewModel;
			PhotoScroller.MyDirPath = dir;
			DataContextChanged += MatchProfileView_DataContextChanged;
			DataContext = item;

			// Close window with ESC
			PreviewKeyDown += (object sender, KeyEventArgs e) =>
			{
				if (e.Key == Key.Escape)
					Close();
			};
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

		private void IGButton_Click(object sender, RoutedEventArgs e)
		{
			var igview = new InstagramView();
			if (DataContext is MatchModel)
			{
				igview.DataContext = (DataContext as MatchModel).Instagram;
				igview.PhotoScroller.DataContext = (DataContext as MatchModel).Instagram.InstagramPhotos;
			}
			else if (DataContext is RecModel)
			{
				igview.DataContext = (DataContext as RecModel).Instagram;
				igview.PhotoScroller.DataContext = (DataContext as RecModel).Instagram.InstagramPhotos;
			}
			igview.PhotoScroller.SerializbleItem = DataContext as ISerializableItem;
			igview.PhotoScroller.MyDirPath = PhotoScroller.MyDirPath;

			igview.Show();
		}
	}
}
