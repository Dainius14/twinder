using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using Twinder.Models.Updates;

namespace Twinder.Converter
{
	class LastMessageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				if (value is MatchModel)
				{
					if (targetType == typeof(string))
					{
						MatchModel match = value as MatchModel;
						if (match.Messages.Count != 0)
						{
							var lastMessage = match.Messages[match.Messages.Count - 1];
							string result = string.Format($"[{lastMessage.SentDateLocal:MM-dd HH:mm}] ");

							if (match.Person.Id == match.Messages[match.Messages.Count - 1].From)
								result += match.Person.Name;
							else
								result += "Me";

							result += string.Format($": {lastMessage.Message}");
							return result;
						}
						return "No messages sent";
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
