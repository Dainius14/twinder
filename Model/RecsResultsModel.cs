using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twinder.Model
{
	public class RecsResultsModel
	{
		// When something goes wrong
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("status")]
		public int Status { get; set; }

		[JsonProperty("results")]
		public List<RecModel> Recommendations { get; set; }
	}
}
