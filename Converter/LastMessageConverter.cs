using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Twinder.Model;

namespace Twinder.Converter
{
	class LastMessageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && value.GetType() == typeof(MatchModel) && targetType == typeof(string))
			{
				MatchModel match = value as MatchModel;
				var lastMessage = match.Messages.LastOrDefault();

				if (lastMessage != null)
				{
					string result = string.Format($"[{lastMessage.SentDate.ToLocalTime():MM-dd HH:mm}] ");
							
					if (match.Person.Id == match.Messages[match.Messages.Count - 1].From)
						result += match.Person.Name;
					else
						result += "Me";

					result += ": " + lastMessage.Message;

					return result;
				}

				return "No messages sent";
			}
			throw new ArgumentException();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
