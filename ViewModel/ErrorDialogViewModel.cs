using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Twinder.ViewModel
{
	public class ErrorDialogViewModel : ViewModelBase
	{
		private string _errorMessage;
		public string ErrorMessage
		{
			get { return _errorMessage; }
			set { Set(ref _errorMessage, value); }
		}

		private string _errorStackTrace;
		public string ErrorStackTrace
		{
			get { return _errorStackTrace; }
			set { Set(ref _errorStackTrace, value); }
		}

		public RelayCommand OkCommand { get; private set; }
		public RelayCommand CopyErrorCommand { get; private set; }

		public ErrorDialogViewModel()
		{
			OkCommand = new RelayCommand(OkButton);
			CopyErrorCommand = new RelayCommand(CopyError);
		}

		private void CopyError()
		{
			Thread thread = new Thread(() => Clipboard.SetText(ErrorMessage + '\n' + ErrorStackTrace));
			thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
			thread.Start();
			thread.Join();
		}

		private void OkButton()
		{
			Application.Current.Shutdown();
		}
	}
}
