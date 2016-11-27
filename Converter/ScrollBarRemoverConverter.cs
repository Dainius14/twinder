using System;
using System.Globalization;
using System.Windows.Data;

namespace Twinder.Converter
{
	class ScrollBarRemoverConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(double) && targetType == typeof(double))
				return (double) value - double.Parse((string) parameter);
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
