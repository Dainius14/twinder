using System;
using System.Globalization;
using System.Windows.Data;
using Twinder.ViewModel;

namespace Twinder.Converter
{
	class FilterMessagedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(MessagedFilter)
				&& parameter != null && parameter.GetType() == typeof(string))
			{
				var paramValue = Enum.Parse(typeof(MessagedFilter), parameter as string);
				return value.Equals(paramValue);
			}
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? Enum.Parse(typeof(MessagedFilter), parameter as string) : Binding.DoNothing;
		}
	}
}
