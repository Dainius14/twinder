using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Model.Updates.Match
{
	public class SimpleMessageModel
	{
		public string From { get; set; }
		
		public string Message { get; set; }
		
		public DateTime SentDate { get; set; }
	}
}
