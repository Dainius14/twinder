using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
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
