using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Models.UserRelated.PhotosRelated
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
