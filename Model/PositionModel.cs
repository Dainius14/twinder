using Newtonsoft.Json;

namespace Twinder.Model
{
	public class PositionModel
	{
		[JsonProperty("Lat")]
		public string Latitude { get; set; }

		[JsonProperty("lon")]
		public string Longtitude { get; set; }

		[JsonProperty("at")]
		public string At { get; set; }
	}
}
