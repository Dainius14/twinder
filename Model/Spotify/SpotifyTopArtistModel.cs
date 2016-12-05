using Newtonsoft.Json;

namespace Twinder.Model.Spotify
{
	public sealed class SpotifyTopArtistModel : SpotifyArtistModel
	{
		[JsonProperty("selected")]
		public bool Selected { get; set; }

		[JsonProperty("top_track")]
		public SpotifyTrackModel TopTrack { get; set; }
	}
}
