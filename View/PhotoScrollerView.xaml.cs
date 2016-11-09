using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Twinder.View
{
	/// <summary>
	/// Interaction logic for PhotoScrollerView.xaml
	/// </summary>
	public partial class PhotoScrollerView : UserControl
	{
		public PhotoScrollerView()
		{
			InitializeComponent();
		}

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

	}
}
