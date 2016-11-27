using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twinder.Converter
{
	class DateTimeToLocalConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(DateTime))
			{
				return ((DateTime) value).ToLocalTime();
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return ((DateTime) value).ToUniversalTime();
			}
			return Binding.DoNothing;
		}
	}
}
