using Microsoft.Maps.MapControl.WPF;
using System.Windows;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class SetLocationView : Window
	{
		public SetLocationView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles adding pushpins to the map
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Map_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Point mousePos = e.GetPosition(this);
			Location pinLocation = map.ViewportPointToLocation(mousePos);

			var viewModel = DataContext as SetLocationViewModel;
			viewModel.MyLatitude = pinLocation.Latitude.ToString();
			viewModel.MyLongtitude = pinLocation.Longitude.ToString();

			Pushpin pin = new Pushpin();
			pin.Location = pinLocation;

			// Removes any existing pin as only one location can be set
			map.Children.Clear();
			map.Children.Add(pin);

			e.Handled = true;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
