using System;
using System.Globalization;
using System.Windows.Data;

namespace Twinder.Converter
{
	class InvertBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(bool) && targetType == typeof(bool))
			{
				return !((bool) value);
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(bool) && targetType == typeof(bool))
			{
				return !((bool) value);
			}
			return Binding.DoNothing;
		}
	}
}
