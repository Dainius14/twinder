using Microsoft.Maps.MapControl.WPF;
using System.Windows;
using Twinder.Helpers;
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class SetLocationView : Window
	{
		public SetLocationView()
		{
			InitializeComponent();

			// Zooms to current user location
			var viewModel = DataContext as SetLocationViewModel;
			var location = new Location(double.Parse(viewModel.MyLatitude), double.Parse(viewModel.MyLongtitude));
			DropPin(location);
			
			// TODO what's the posMajor?
			//location = new Location(double.Parse(TinderHelper.User.PosMajor.Latitude.Replace('.', ',')), double.Parse(TinderHelper.User.PosMajor.Longtitude.Replace('.', ',')));
			//DropPin(location);
			map.Center = location;
			map.ZoomLevel = 10;
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

			DropPin(pinLocation);

			e.Handled = true;
		}

		private void DropPin(Location pinLocation)
		{
			Pushpin pin = new Pushpin();
			pin.Location = pinLocation;

			// Removes any existing pin as only one location can be set
			map.Children.Clear();
			map.Children.Add(pin);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
