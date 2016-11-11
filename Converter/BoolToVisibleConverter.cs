using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Twinder.Converter
{
	class BoolToVisibleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				if (value is bool)
				{
					if (targetType == typeof(Visibility))
					{
						if ((bool) value)
							return Visibility.Visible;
						else
							return
								Visibility.Collapsed;
					}
					throw new ArgumentException("Wrong target type.");
				}
				throw new ArgumentException("Wrong input type.");
			}
			throw new ArgumentNullException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
