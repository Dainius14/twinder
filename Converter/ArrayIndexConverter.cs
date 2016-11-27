using System;
using System.Globalization;
using System.Windows.Data;

namespace Twinder.Converter
{
	class ArrayIndexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(int))
				return (int) value + 1;
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
