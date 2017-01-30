using System;
using System.Windows;
using Twinder.ViewModel;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for ErrorDialogView.xaml
	/// </summary>
	public partial class ErrorDialogView : Window
	{		
		public ErrorDialogView(Exception exception)
		{
			InitializeComponent();

			var viewModel = DataContext as ErrorDialogViewModel;
			viewModel.ErrorMessage = exception.Message;
			viewModel.ErrorStackTrace = exception.StackTrace;
		}
	}
}
