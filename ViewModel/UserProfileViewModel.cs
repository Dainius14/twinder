using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Twinder.Helpers;
using Twinder.Models;

namespace Twinder.ViewModel
{
	public class UserProfileViewModel : ViewModelBase
	{

		private UserModel _user;
		public UserModel User
		{
			get { return _user; }
			set
			{
				Set(ref _user, value);
				SetUp();
			}
		}

		private string _bio;
		public string Bio
		{
			get { return _bio; }
			set { Set(ref _bio, value); }
		}

		public int _minAge;
		public int MinAge
		{
			get { return _minAge; }
			set { Set(ref _minAge, value); }
		}

		public int _maxAge;
		public int MaxAge
		{
			get { return _maxAge; }
			set { Set(ref _maxAge, value); }
		}
		public int _distanceKm;
		public int Distance
		{
			get { return _distanceKm; }
			set { Set(ref _distanceKm, value); }
		}

		private Gender _interestedIn;
		public Gender InterestedIn
		{
			get { return _interestedIn; }
			set { Set(ref _interestedIn, value); }
		}

		public RelayCommand SaveCommand { get; private set; }

		public UserProfileViewModel()
		{

			SaveCommand = new RelayCommand(Save, CanSave);

		}
		/// <summary>
		/// Sets editable fiels to seperate properties
		/// </summary>
		private void SetUp()
		{
			Bio = User.Bio;
			MinAge = User.AgeFilterMin;
			MaxAge = User.AgeFilterMax;
			Distance = User.DistanceFilter;

			if (User.InterestedIn.Count == 2)
				InterestedIn = Gender.Both;
			else
			{
				InterestedIn = User.InterestedIn[0];
			}
		}

		#region Save command
		private async void Save()
		{
			var activeTime = await TinderHelper.UpdateUser(Bio, MinAge, MaxAge, Distance, InterestedIn);

			if (activeTime != default(DateTime))
			{
				User.ActiveTime = activeTime;
				User.Bio = Bio;
				User.AgeFilterMin = MinAge;
				User.AgeFilterMax = MaxAge;
				User.DistanceFilter = Distance;
				User.InterestedIn.Clear();
				if (InterestedIn == Gender.Both)
				{
					User.InterestedIn.Add(Gender.Man);
					User.InterestedIn.Add(Gender.Woman);
				}
				else
					User.InterestedIn.Add(InterestedIn);

			}
			else
				MessageBox.Show("Now that's a pity. There was an error.");


		}

		private bool CanSave()
		{
			return true;
		}
		#endregion 
	}
}
