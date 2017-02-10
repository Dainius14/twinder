using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twinder.Converter
{
	class BoolToVisibleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(bool) && targetType == typeof(Visibility))
			{
				// Inverse
				if (parameter != null)
				{
					if ((bool) value)
						return Visibility.Collapsed;
					else
						return Visibility.Visible;
				}

				if ((bool) value)
					return Visibility.Visible;
				else
					return Visibility.Collapsed;
			}
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
