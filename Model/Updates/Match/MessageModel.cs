using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Models.Updates
{
	public sealed class MessageModel 
	{
		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("match_id")]
		public string MatchId { get; set; }

		[JsonProperty("to")]
		public string To { get; set; }

		[JsonProperty("from")]
		public string From { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("sent_date")]
		public DateTime SentDate { get; set; }

		[JsonProperty("created_date")]
		public DateTime CreatedDate { get; set; }

		[JsonProperty("timestamp")]
		public string TimeStamp { get; set; }

		public override bool Equals(object obj)
		{
			return Id == (obj as MessageModel).Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}
