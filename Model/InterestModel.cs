using Newtonsoft.Json;
using System;

namespace Twinder.Model
{
	public sealed class InterestModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("created_time")]
		public DateTime CreatedTime { get; set; }
	}
}
