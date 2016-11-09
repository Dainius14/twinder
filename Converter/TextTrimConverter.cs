using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Twinder.Converter
{
	class TextTrimConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				if (targetType == typeof(string))
				{
					string text = (string) value;
					// Removes multiple spaces
					text = new Regex(@"(\s\s+)|(\t|\n|\r)").Replace(text, " ");
					text.Trim();
					return text;
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
