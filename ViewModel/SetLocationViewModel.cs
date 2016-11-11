using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Twinder.Helpers;
using Twinder.View;

namespace Twinder.ViewModel
{
	public class SetLocationViewModel : ViewModelBase
	{
		public RelayCommand SetLocationCommand { get; set; }
		public RelayCommand AutoDetectCommand { get; set; }

		private string _myLatitude;
		public string MyLatitude
		{
			get { return _myLatitude; }
			set { Set(ref _myLatitude, value); }
		}

		private string _myLongtitude;
		public string MyLongtitude
		{
			get { return _myLongtitude; }
			set { Set(ref _myLongtitude, value); }
		}

		private bool _isAutoDetecting;
		public bool IsAutoDetecting
		{
			get { return _isAutoDetecting; }
			set { Set(ref _isAutoDetecting, value); }
		}

		private bool _isAutoDetectingError;
		public bool IsAutoDetectingError
		{
			get { return _isAutoDetectingError; }
			set { Set(ref _isAutoDetectingError, value); }
		}


		public SetLocationViewModel()
		{
			SetLocationCommand = new RelayCommand(SetLocation);
			AutoDetectCommand = new RelayCommand(AutoDetect);

			MyLatitude = Properties.Settings.Default.latitude;
			MyLongtitude = Properties.Settings.Default.longtitude;
			IsAutoDetecting = false;
			IsAutoDetectingError = false;
		}

		private void SetLocation()
		{
			Properties.Settings.Default["latitude"] = MyLatitude;
			Properties.Settings.Default["longtitude"] = MyLongtitude;
			Properties.Settings.Default.Save();
			TinderHelper.PingLocation(MyLatitude, MyLongtitude);
		}

		private void AutoDetect()
		{
			// Doesn't work so far, I don't know why
			GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
			IsAutoDetecting = true;
			Task<bool> myTask = new Task<bool>(() =>
			{
				if (watcher.TryStart(false, TimeSpan.FromSeconds(10)))
				{
					MyLatitude = watcher.Position.Location.Latitude.ToString();
					MyLongtitude = watcher.Position.Location.Longitude.ToString();
					return true;
				}
				return false;

			});

			myTask.Start();
			IsAutoDetectingError = myTask.Result;

			IsAutoDetecting = false;
		}

	}
}
