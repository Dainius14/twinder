using Newtonsoft.Json;

namespace Twinder.Model.Photos
{
	public sealed class InstagramPhoto
	{
		[JsonProperty("image")]
		public string Image { get; set; }

		[JsonProperty("localImage")]
		public string LocalImage { get; set; }

		[JsonProperty("localThumbnail")]
		public string LocalThumbnail { get; set; }

		[JsonProperty("thumbnail")]
		public string Thumbnail { get; set; }

		[JsonProperty("ts")]
		public string Ts { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }
	}
}
