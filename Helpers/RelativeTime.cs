using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Helpers
{
	/*
	 * Code from http://dotnetthoughts.net/time-ago-function-for-c/
	 */

	static class RelativeTime
	{
		// TODO deal with localization maybe. or maybe not
		public static string TimeAgo(this DateTime dateTime)
		{
			string result = string.Empty;
			var timeSpan = DateTime.Now.Subtract(dateTime);

			if (timeSpan <= TimeSpan.FromSeconds(10))
			{
				result = string.Format("moments ago", timeSpan.Seconds);
			}

			else if (timeSpan <= TimeSpan.FromSeconds(60))
			{
				result = string.Format("{0} seconds ago", timeSpan.Seconds);
			}
			else if (timeSpan <= TimeSpan.FromMinutes(60))
			{
				result = timeSpan.Minutes > 1 ?
					String.Format("{0} minutes ago", timeSpan.Minutes) :
					"a minute ago";
			}
			else if (timeSpan <= TimeSpan.FromHours(24))
			{
				result = timeSpan.Hours > 1 ?
					String.Format("{0} hours ago", timeSpan.Hours) :
					"an hour ago";
			}
			else if (timeSpan <= TimeSpan.FromDays(30))
			{
				result = timeSpan.Days > 1 ?
					String.Format("{0} days ago", timeSpan.Days) :
					"yesterday";
			}
			else if (timeSpan <= TimeSpan.FromDays(365))
			{
				result = timeSpan.Days > 30 * 2?
					String.Format("{0} months ago", timeSpan.Days / 30) :
					"a month ago";
			}
			else
			{
				result = timeSpan.Days > 365 * 2 ?
					String.Format("{0} years and ", timeSpan.Days / 365) :
					"a year and ";

				result += timeSpan.Days % 365 > 30 * 2?
					String.Format("{0} months ago", timeSpan.Days % 365 / 30) :
					"a month ago";
			}

			return result;
		}
	}
}
