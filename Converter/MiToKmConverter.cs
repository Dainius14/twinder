using System;
using System.Globalization;
using System.Windows.Data;

namespace Twinder.Converter
{
	class MiToKmConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return (int) Math.Round((int) value * 1.6);
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return (int) Math.Round((double) value / 1.6);
			}
			return Binding.DoNothing;
		}
	}
}
