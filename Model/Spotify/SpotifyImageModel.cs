using Newtonsoft.Json;

namespace Twinder.Model.Spotify
{
	public class SpotifyImageModel
	{
		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("width")]
		public int Width { get; set; }

	}
}
