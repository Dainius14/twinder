using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.Models.Updates;

namespace Twinder.ViewModel
{
	public class RecommendationsViewModel : ViewModelBase
	{
		private ObservableCollection<RecommendationModel> _recommendations;
		public ObservableCollection<RecommendationModel> Recommendations
		{
			get { return _recommendations; }
			set
			{
				// If recommendations are set for the first time
				if (_recommendations == null)
				{
					_selectedIndex = 0;
					SelectedRec = value[0];
				}

				Set(ref _recommendations, value); }
		}

		private RecommendationModel _selectedRec;
		public RecommendationModel SelectedRec
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
		
		#region Select Previous command
		/// <summary>
		/// Selects previous recommendation
		/// </summary>
		private void SelectPrevious()
		{
			if (SelectedIndex == 0)
				SelectedIndex = Recommendations.Count - 1;
			else
				SelectedIndex--;

			SelectedRec = Recommendations[_selectedIndex];
		}
		#endregion

		#region Select Next command
		/// <summary>
		/// Selects next recommendation
		/// </summary>
		private void SelectNext()
		{
			if (SelectedIndex == Recommendations.Count - 1)
				SelectedIndex = 0;
			else
				SelectedIndex++;

			SelectedRec = Recommendations[SelectedIndex];
		}
		#endregion

		#region Pass command
		/// <summary>
		/// Passes the selected recommendation
		/// </summary>
		private void Pass()
		{
			TinderHelper.PassRecommendation(SelectedRec.Id);
			RemoveSelectedRecommendation();
		}
		#endregion

		#region Like command
		/// <summary>
		/// Likes the selected recommendation
		/// </summary>
		/// <param name="superLike">True if a superlike should be send, default is false</param>
		private void Like(bool superLike = false)
		{
			var match = TinderHelper.LikeRecommendation(SelectedRec.Id, superLike);

			if (match != null)
			{
				MessageBox.Show("It's a match!");
			}

			RemoveSelectedRecommendation();
		}
		#endregion
		

		#region Like All command
		/// <summary>
		/// Likes all of the recommendations
		/// </summary>
		private async void LikeAll()
		{
			for (int i = 0; i < Recommendations.Count; i++)
			{
				var rec = Recommendations[i];

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
		private void RemoveRecomendations(RecommendationModel rec)
		{
			if (rec == SelectedRec)
				RemoveSelectedRecommendation();
			Recommendations.Remove(rec);
		}

		/// <summary>
		/// Removes recommendation which is currently selected
		/// </summary>
		private void RemoveSelectedRecommendation()
		{
			Recommendations.Remove(SelectedRec);
			// If it was the last item, selects a new last item
			if (SelectedIndex >= Recommendations.Count)
			{
				SelectedIndex = Recommendations.Count - 1;
				if (SelectedIndex >= 0)
					SelectedRec = Recommendations[SelectedIndex];
			}
			// If no more recommendations are left, removes selection
			else if (Recommendations.Count == 0)
				SelectedRec = null;
			else
				SelectedRec = Recommendations[SelectedIndex];
		}
	}
}
