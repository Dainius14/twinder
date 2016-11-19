using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.Models;
using Twinder.Models.Updates;

namespace Twinder.ViewModel
{
	public class RecommendationsViewModel : ViewModelBase
	{
		public event LoadingStateChangeHandler LoadingStateChange;
		public delegate void LoadingStateChangeHandler(object sender, RecsLoadingStateEventArgs e);

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


		public RecommendationsViewModel()
		{
			SelectPreviousCommand = new RelayCommand(SelectPrevious);
			SelectNextCommand = new RelayCommand(SelectNext);
			PassCommand = new RelayCommand(Pass);
			LikeCommand = new RelayCommand<bool>(param => Like(false));
			SuperLikeCommand = new RelayCommand<bool>(param => Like(true));
			LikeAllCommand = new RelayCommand(LikeAll);
		}

		/// <summary>
		/// Sets RecList property to input, adds event listener and invokes LoadingStateChange event
		/// </summary>
		/// <param name="recList"></param>
		public void SetRecommendations(ObservableCollection<RecModel> recList)
		{
			RecsLoadingStateEventArgs args = new RecsLoadingStateEventArgs();
			if (recList != null)
			{
				RecList = recList;
				RecList.CollectionChanged += GetMoreRecs;
				SelectedRec = RecList[0];
				SelectedIndex = 0;

				args.RecsStatus = RecsStatus.Okay;
			}
			else
				args.RecsStatus = RecsStatus.Exhausted;

			LoadingStateChange.Invoke(this, args);
		}

		public async Task<bool> GetRecommendations()
		{
			RecsLoadingStateEventArgs args = new RecsLoadingStateEventArgs();
			args.RecsStatus = RecsStatus.Getting;

			LoadingStateChange.Invoke(this, args);
			var recs = await TinderHelper.GetRecommendations();
			if (recs.Recommendations != null)
			{
				var recList = new ObservableCollection<RecModel>(recs.Recommendations);
				SetRecommendations(recList);
				
				return true;
			}
			args.RecsStatus = RecsStatus.Exhausted;
			LoadingStateChange.Invoke(this, args);
			return false;
		}
		
		#region Select Previous command
		/// <summary>
		/// Selects previous recommendation
		/// </summary>
		private void SelectPrevious()
		{
			if (RecList.Count > 0)
			{
				if (SelectedIndex == 0)
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
		private void Pass()
		{
			if (SelectedRec != null)
			{
				TinderHelper.PassRecommendation(SelectedRec.Id);
				RemoveSelectedRecommendation();
			}
		}
		#endregion

		#region Like command
		/// <summary>
		/// Likes the selected recommendation
		/// </summary>
		/// <param name="superLike">True if a superlike should be send, default is false</param>
		private async void Like(bool superLike = false)
		{
			if (SelectedRec != null)
			{
				var match = await TinderHelper.LikeRecommendation(SelectedRec.Id, superLike);

				// If the method returns a non-null value, it means it's a match
				if (match != null)
				{
					Messenger.Default.Send("", MessengerToken.ForceUpdate);
					MessageBox.Show("It's a match!");
				}

				RemoveSelectedRecommendation();
			}
		}
		#endregion


		#region Like All command
		/// <summary>
		/// Likes all of the recommendations
		/// </summary>
		private async void LikeAll()
		{
			for (int i = 0; i < RecList.Count; i++)
			{
				var rec = RecList[i];

				if (OnlyWithoutDescription && string.IsNullOrEmpty(rec.Bio))
				{
					await TinderHelper.LikeRecommendation(rec.Id);
					RemoveRecomendations(rec);
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
			if (rec == SelectedRec)
				RemoveSelectedRecommendation();
			RecList.Remove(rec);
		}

		/// <summary>
		/// Removes recommendation which is currently selected
		/// </summary>
		private void RemoveSelectedRecommendation()
		{
			RecList.Remove(SelectedRec);
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

		/// <summary>
		/// When there no more recommendations - tries to get more
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void GetMoreRecs(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (RecList.Count == 0)
			{
				await GetRecommendations();
			}
		}
	}

	public class RecsLoadingStateEventArgs : EventArgs
	{
		public RecsStatus RecsStatus { get; set; }
	}
}
