using Newtonsoft.Json;

namespace Twinder.Model.Photos
{
	public sealed class ProcessedFileModel
	{
		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
