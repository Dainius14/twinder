using Newtonsoft.Json;
using System.Collections.Generic;

namespace Twinder.Model.Spotify
{
	public sealed class SpotifyAlbumModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("images")]
		public List<SpotifyImageModel> Images { get; set; }
	}
}
