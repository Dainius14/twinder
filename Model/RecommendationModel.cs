using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Twinder.Models.UserRelated;
using Twinder.Models.UserRelated.PhotosRelated;

namespace Twinder.Model
{
	public class RecommendationModel
	{
		[JsonProperty("distance_mi")]
		public int DistanceMiles { get; set; }
		public int DistanceKilometers { get { return (int) Math.Round(DistanceMiles * 1.609); } }

		[JsonProperty("connection_count")]
		public int ConnectionCount { get; set; }

		[JsonProperty("common_like_count")]
		public int CommonLikeCount { get; set; }

		[JsonProperty("common_friend_count")]
		public int CommonFriendCount { get; set; }

		[JsonProperty("common_likes")]
		public ObservableCollection<string> CommonLikes { get; set; }

		[JsonProperty("common_friends")]
		public ObservableCollection<string> CommonFriends { get; set; }

		[JsonProperty("content_hash")]
		public string ContentHash { get; set; }

		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("badges")]
		public dynamic Badges { get; set; }

		[JsonProperty("bio")]
		public string Bio { get; set; }

		[JsonProperty("birth_date")]
		public DateTime BirthDate { get; set; }
		public int Age { get { return (int) (DateTime.Now - BirthDate).TotalDays / 365; } }

		[JsonProperty("gender")]
		public int Gender { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("ping_time")]
		public DateTime PingTime { get; set; }
		public DateTime PingTimeLocal { get { return PingTime.ToLocalTime(); } }

		[JsonProperty("photos")]
		public ObservableCollection<PhotoModel> Photos { get; set; }

		[JsonProperty("instagram")]
		public InstagramModel Instagram { get; set; }

		[JsonProperty("jobs")]
		public dynamic Jobs { get; set; }

		[JsonProperty("schools")]
		public ObservableCollection<SchoolModel> Schools { get; set; }

		[JsonProperty("teaser")]
		public dynamic Teaser { get; set; }

		[JsonProperty("birth_date_info")]
		public string BirthDateInfo { get; set; }

	}
}
