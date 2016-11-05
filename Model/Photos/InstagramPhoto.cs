using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Model.Photos
{
	public class InstagramPhoto
	{
		[JsonProperty("image")]
		public string Image { get; set; }

		[JsonProperty("thumbnail")]
		public string Thumbnail { get; set; }

		[JsonProperty("ts")]
		public string Ts { get; set; }

		[JsonProperty("link")]
		public string Link { get; set; }
	}
}
