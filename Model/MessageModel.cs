using Newtonsoft.Json;
using System;

namespace Twinder.Model
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
			var otherObj = (obj as MessageModel);
			if (otherObj == null)
				return false;
			return Id == otherObj.Id;
		}

		public override int GetHashCode()
		{
			if (Id != null)
				return Id.GetHashCode();
			return base.GetHashCode();
		}
	}
}
