using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Twinder.Model.Photos
{
	public sealed class PhotoModel
	{
		// TODO wtf is these
		[JsonProperty("selectRate")]
		public string SelectRate { get; set; }

		[JsonProperty("successRate")]
		public string SuccessRate { get; set; }
		

		[JsonProperty("main")]
		public string Main { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("xdistance_percent")]
		public string XDistancePercent { get; set; }

		[JsonProperty("ydistance_percent")]
		public string YDistancePercent { get; set; }

		[JsonProperty("xoffset_percent")]
		public string XOffsetPercent { get; set; }

		[JsonProperty("yoffset_percent")]
		public string YOffsetPercent { get; set; }

		[JsonProperty("fileName")]
		public string FileName { get; set; }

		[JsonProperty("fbId")]
		public string FbId { get; set; }

		[JsonProperty("extension")]
		public string Extension { get; set; }

		[JsonProperty("processedFiles")]
		public ObservableCollection<ProcessedFileModel> ProcessedFiles { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
