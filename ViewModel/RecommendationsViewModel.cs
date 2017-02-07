using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.View;

namespace Twinder.ViewModel
{
	public class RecommendationsViewModel : ViewModelBase
	{

		private ObservableCollection<RecModel> _recList;
		public ObservableCollection<RecModel> RecList
		{
			get { return _recList; }
			set { Set(ref _recList, value); }
		}

		private RecModel _selectedRec;
		public RecModel SelectedRec
		{
			get { return _selectedRec; }
			set { Set(ref _selectedRec, value); }
		}

		private int _selectedIndex;
		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set { Set(ref _selectedIndex, value); }
		}

		private int _superLikesLeft;
		public int SuperLikesLeft
		{
			get { return _superLikesLeft; }
			set { Set(ref _superLikesLeft, value); }
		}

		private int _likesLeft;
		public int LikesLeft
		{
			get { return _likesLeft; }
			set { Set(ref _likesLeft, value); }
		}
		
		private bool _canSuperLike;
		public bool CanSuperLike
		{
			get { return _canSuperLike; }
			set { Set(ref _canSuperLike, value); }
		}

		private bool _canLike;
		public bool CanLike
		{
			get { return _canLike; }
			set { Set(ref _canLike, value); }
		}

		public RelayCommand SelectPreviousCommand { get; private set; }
		public RelayCommand SelectNextCommand { get; private set; }
		public RelayCommand PassCommand { get; private set; }
		public RelayCommand<bool> LikeCommand { get; private set; }
		public RelayCommand<bool> SuperLikeCommand { get; private set; }
		public RelayCommand LikeAllCommand { get; private set; }

		private bool _onlyWithoutDescription;
		public bool OnlyWithoutDescription
		{
			get { return _onlyWithoutDescription; }
			set { Set(ref _onlyWithoutDescription, value); }
		}

		public RecommendationsView RecsView { get; internal set; }

		public RecommendationsViewModel()
		{
			CanSuperLike = SerializationHelper.GetSuperLikeReset() < DateTime.Now;
			LikesLeft = SerializationHelper.GetLikesLeft();
			SuperLikesLeft = SerializationHelper.GetSuperLikesLeft();

			SelectPreviousCommand = new RelayCommand(SelectPrevious);
			SelectNextCommand = new RelayCommand(SelectNext);
			PassCommand = new RelayCommand(Pass);
			LikeCommand = new RelayCommand<bool>(async param => await Like(false));
			SuperLikeCommand = new RelayCommand<bool>(async param => await Like(true), param => CanSuperLike);
			LikeAllCommand = new RelayCommand(async () => await LikeAll());

		}

		/// <summary>
		/// Sets RecList property to input, adds event listener and invokes LoadingStateChange event
		/// </summary>
		/// <param name="recList"></param>
		public void SetRecommendations(ObservableCollection<RecModel> recList)
		{
			if (recList != null)
			{
				// First time 
				if (RecList == null)
				{
					RecList = recList;
					RecList.CollectionChanged += RecList_CollectionChanged;
					SelectedRec = RecList[0];
					SelectedIndex = 0;
				}
			}
		}
		
		#region Select Previous command
		/// <summary>
		/// Selects previous recommendation
		/// </summary>
		private void SelectPrevious()
		{
			if (RecList.Count > 0)
			{
				if (SelectedIndex <= 0)
					SelectedIndex = RecList.Count - 1;
				else
					SelectedIndex--;

				SelectedRec = RecList[SelectedIndex];
			}
		}
		#endregion

		#region Select Next command
		/// <summary>
		/// Selects next recommendation
		/// </summary>
		private void SelectNext()
		{
			if (RecList.Count > 0)
			{
				if (SelectedIndex == RecList.Count - 1)
					SelectedIndex = 0;
				else
					SelectedIndex++;

				SelectedRec = RecList[SelectedIndex];
			}
		}
		#endregion

		#region Pass command
		/// <summary>
		/// Passes the selected recommendation
		/// </summary>
		private async void Pass()
		{
			if (SelectedRec != null)
			{
				try
				{
					await TinderHelper.PassRecommendation(SelectedRec.Id);
					var matchToDelete = SelectedRec;
					RemoveRecomendations(SelectedRec);
					RecsView.UpdateLayout();

					SerializationHelper.MoveRecToPassed(matchToDelete);
				}
				catch (TinderRequestException e)
				{
					MessageBox.Show(e.Message);
				}
			}
		}
		#endregion

		#region Like command
		/// <summary>
		/// Likes the selected recommendation
		/// </summary>
		/// <param name="superLike">True if a superlike should be send, default is false</param>
		private async Task Like(bool superLike = false, bool showMessage = true)
		{
			if (SelectedRec != null)
			{
				try
				{
					var likesent = await TinderHelper.LikeRecommendation(SelectedRec.Id, superLike);
					
					// It was a super like, update if neccesarry
					if (superLike)
					{
						if (likesent.SuperLikes.Remaining == 0)
						{
							SerializationHelper.UpdateSuperLikes(likesent.SuperLikes.ResetsAt, likesent.SuperLikes.Remaining);
							CanSuperLike = false;
						}
					}

					var matchToMove = SelectedRec;
					RemoveRecomendations(SelectedRec);
					RecsView.UpdateLayout();

					// If the method returns a non-null value, it means it's a match
					if (likesent.Match != null)
					{
						SerializationHelper.MoveRecToMatches(matchToMove);
						Messenger.Default.Send("", MessengerToken.ForceUpdate);

						if (showMessage)
							MessageBox.Show($"It's a match! {likesent.LikesRemainig} likes left." );
					}
					else
						SerializationHelper.MoveRecToPending(matchToMove);

				}
				catch (TinderRequestException e)
				{
					MessageBox.Show(e.Message);
				}
			}
		}
		#endregion

		#region Like All command
		/// <summary>
		/// Likes all of the recommendations
		/// </summary>
		private async Task LikeAll()
		{
			for (int i = 0; i < RecList.Count; i++)
			{
				if (!OnlyWithoutDescription || (OnlyWithoutDescription && string.IsNullOrEmpty(SelectedRec.Bio)))
				{
					await Like(showMessage: false);
					i--;
				}
			}
		}
		#endregion

		/// <summary>
		/// Removes recommendation
		/// </summary>
		/// <param name="rec">Recommendation to remove</param>
		private void RemoveRecomendations(RecModel rec)
		{
			RecList.Remove(rec);

			// Since the recommendation was currently selected, we have to change it with another one
			if (rec == SelectedRec)
			{
				// If it was the last item, selects a new last item
				if (SelectedIndex >= RecList.Count)
				{
					SelectedIndex = RecList.Count - 1;
					if (SelectedIndex >= 0)
						SelectedRec = RecList[SelectedIndex];
				}
				// If no more recommendations are left, removes selection
				if (RecList.Count == 0)
					SelectedRec = null;
				else
					SelectedRec = RecList[SelectedIndex];
			}

			if (RecList.Count == 0)
			{
				Messenger.Default.Send("", MessengerToken.GetMoreRecs);
				// Close the god damn window, less hassle
				RecsView.Close();
			}

		}

		/// <summary>
		/// Is executed when reclist gets new recommendations added
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RecList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{

		}
	}
	
}
