using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twinder.Model.Spotify
{
	public class SpotifyTrackModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("artists")]
		public List<SpotifyArtistModel> Artists{ get; set; }

		[JsonProperty("preview_url")]
		public string PreviewUrl { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("album")]
		public SpotifyAlbumModel Album { get; set; }

		[JsonProperty("uri")]
		public string Uri { get; set; }
	}
}
