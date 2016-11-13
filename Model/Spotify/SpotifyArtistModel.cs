using Newtonsoft.Json;

namespace Twinder.Model.Spotify
{
	public class SpotifyArtistModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
