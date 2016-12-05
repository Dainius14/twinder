using Newtonsoft.Json;

namespace Twinder.Model
{
	public sealed class TeaserModel
	{
		[JsonProperty("string")]
		public string String { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}
