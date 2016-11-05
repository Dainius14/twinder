using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Model
{
	public class InstagramModel
	{
		[JsonProperty("last_fetch_time")]
		public DateTime LastFetchTime { get; set; }

		[JsonProperty("completed_initial_fetch")]
		public bool CompletedInitialFetch { get; set; }

		[JsonProperty("photos")]
		public ObservableCollection<InstagramModel> InstagramPhotos { get; set; }

		[JsonProperty("media_count")]
		public int MediaCount { get; set; }

		[JsonProperty("profile_picture")]
		public string ProfilePicture { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

	}
}
