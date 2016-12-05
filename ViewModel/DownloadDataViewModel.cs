using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using Twinder.Helpers;
using Twinder.View;

namespace Twinder.ViewModel
{
	public class DownloadDataViewModel : ViewModelBase
	{
		private int _currentProgress;
		public int CurrentProgress
		{
			get { return _currentProgress; }
			set { Set(ref _currentProgress, value); }
		}

		private int _currentItem;
		public int CurrentItem
		{
			get { return _currentItem; }
			set { Set(ref _currentItem, value); }
		}

		private int _currentTotalCount;
		public int CurrentTotalCount
		{
			get { return _currentTotalCount; }
			set { Set(ref _currentTotalCount, value); }
		}

		private string _currentStatus;
		public string CurrentStatus
		{
			get { return _currentStatus; }
			set { Set(ref _currentStatus, value); }
		}

		private bool _progressCountVisibility;
		public bool ProgressCountVisibility
		{
			get { return _progressCountVisibility; }
			set { Set(ref _progressCountVisibility, value); }
		}
		
		public RelayCommand CancelCommand { get; private set; }

		public SerializationPacket Packet { get; set; }

		public DownloadDataView MyView { get; set; }
		public BackgroundWorker Worker { get; private set; }
		private double _totalCount;
		private double _totalItems;

		public DownloadDataViewModel()
		{
			Worker = new BackgroundWorker();
			ProgressCountVisibility = true;
			Worker.WorkerReportsProgress = true;
			Worker.WorkerSupportsCancellation = true;
			Worker.DoWork += Worker_DoWork;
			Worker.ProgressChanged += Worker_ProgressChanged;

			CancelCommand = new RelayCommand(Cancel);
		}
		
		private void Cancel()
		{
			Worker.Dispose();
		}

		public void StartDownload()
		{
			_totalItems = 0;
			_totalCount = 0;
			_totalCount += Packet.User != null ? 1 : 0;
			_totalCount += Packet.RecList != null ? Packet.RecList.Count : 0;
			_totalCount += Packet.MatchList != null ? Packet.MatchList.Count : 0;
			Worker.RunWorkerAsync();
		}

		/// <summary>
		/// Loads every serializer one after another
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			if (Packet.User != null)
			{
				// It doens't make sense to see progress for one user
				ProgressCountVisibility = false;
				CurrentItem = 0;
				CurrentStatus = Properties.Resources.download_data_user;
				SerializationHelper.SerializeUser(Packet.User, Worker);
			}

			if (Packet.RecList != null)
			{
				ProgressCountVisibility = true;
				CurrentItem = 1;
				CurrentStatus = Properties.Resources.download_data_recs;
				CurrentTotalCount = Packet.RecList.Count;
				SerializationHelper.SerializeRecList(Packet.RecList, Worker);
			}

			if (Packet.MatchList != null)
			{
				CurrentItem = 1;
				CurrentStatus = Properties.Resources.download_data_matches;
				CurrentTotalCount = Packet.MatchList.Count;
				SerializationHelper.SerializeMatchList(Packet.MatchList, Worker);
			}

			// Only here the baby is done
			Properties.Settings.Default["SerializationComplete"] = true;
			Properties.Settings.Default.Save();
			e.Result = true;
		}

		/// <summary>
		/// Updates properties on progress change
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			_totalItems += e.ProgressPercentage;
			CurrentItem = (int) e.UserState;
			CurrentProgress = (int) (Math.Round(_totalItems / _totalCount * 100));
		}

	}
}
