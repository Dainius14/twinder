using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Twinder.Model;

namespace Twinder.Converter
{
	class ChatBubbleTailConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && parameter != null)
			{
				var from = (string) value;
				var match = parameter as MatchModel;
				if (from == match.Id)
					return Visibility.Visible;
			}
			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
