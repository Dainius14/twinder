using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twinder.Converter
{
	class ScrollBarRemoverConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.GetType() == typeof(double))
				return (double) value - double.Parse((string) parameter);
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.GetType() == typeof(int))
				return (int) value + double.Parse((string) parameter);
			return value;
		}
	}
}
