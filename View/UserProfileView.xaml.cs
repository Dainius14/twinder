using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Twinder.Model;	
using Twinder.ViewModel;

namespace Twinder.View
{
	public partial class UserProfileView : Window
	{
		public UserProfileView(UserModel user)
		{
			InitializeComponent();
			UserProfileViewModel viewModel = DataContext as UserProfileViewModel;
			viewModel.User = user;
		}
		
		private void MinAge_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (MinAge_Slider != null && MaxAge_Slider != null)
				if (e.NewValue > MaxAge_Slider.Value && MaxAge_Slider.Value != 0)
				{
					MinAge_Slider.Value = MaxAge_Slider.Value;
					e.Handled = true;
				}
		}

		private void MaxAge_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (MinAge_Slider != null && MaxAge_Slider != null)
				if (e.NewValue < MinAge_Slider.Value)
				{
					MaxAge_Slider.Value = MinAge_Slider.Value;
					e.Handled = true;
				}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
