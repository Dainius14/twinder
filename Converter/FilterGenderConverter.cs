using System;
using System.Globalization;
using System.Windows.Data;
using Twinder.Model;
using Twinder.ViewModel;

namespace Twinder.Converter
{
	class FilterGenderConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(Gender)
				&& parameter != null && parameter.GetType() == typeof(string))
			{
				var paramValue = Enum.Parse(typeof(Gender), parameter as string);
				return value.Equals(paramValue);
			}
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? Enum.Parse(typeof(Gender), parameter as string) : Binding.DoNothing;
		}
	}
}
