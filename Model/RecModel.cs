using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Twinder.Model.Auth.User;
using Twinder.Model.Spotify;
using Twinder.Models.UserRelated;
using Twinder.Models.UserRelated.PhotosRelated;

namespace Twinder.Model
{
	public class RecModel
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

		[JsonProperty("common_interests")]
		public ObservableCollection<string> CommonInterests { get; set; }

		[JsonProperty("uncommon_interests")]
		public ObservableCollection<string> UncommonInterests { get; set; }

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
		public int Age { get { return (int) (DateTime.Now - BirthDate).TotalDays / 365 - 1; } }

		[JsonProperty("gender")]
		public int Gender { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("ping_time")]
		public DateTime PingTime { get; set; }

		[JsonProperty("photos")]
		public ObservableCollection<PhotoModel> Photos { get; set; }

		[JsonProperty("instagram")]
		public InstagramModel Instagram { get; set; }

		[JsonProperty("jobs")]
		public List<JobModel> Jobs { get; set; }

		[JsonProperty("schools")]
		public List<SchoolModel> Schools { get; set; }

		[JsonProperty("teaser")]
		public TeaserModel Teaser { get; set; }

		[JsonProperty("birth_date_info")]
		public string BirthDateInfo { get; set; }

		[JsonProperty("s_number")]
		public string SNumber { get; set; }

		[JsonProperty("spotify_top_artists")]
		public List<SpotifyTopArtistModel> SpotifyTopArtists { get; set; }

		[JsonProperty("spotify_theme_track")]
		public SpotifyTrackModel SpotifyThemeTrack { get; set; }

		[JsonProperty("is_traveling")]
		public bool IsTraveling { get; set; }
	}
}
