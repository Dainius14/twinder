using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Twinder.Converter
{
	class TextTrimConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(string) && targetType == typeof(string))
			{
				string text = (string) value;
				// Removes multiple spaces
				text = new Regex(@"(\s\s+)|(\t|\n|\r)").Replace(text, " ");
				text.Trim();
				return text;
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
