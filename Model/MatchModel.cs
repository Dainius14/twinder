using BinaryAnalysis.UnidecodeSharp;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Twinder.Helpers;
using Twinder.Model.Photos;
using Twinder.Model.Spotify;
using System.Threading.Tasks;
using Unidecode.NET;

namespace Twinder.Model
{
	public class MatchModel
	{
		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("closed")]
		public bool Closed { get; set; }

		[JsonProperty("connection_count")]
		public int ConnectionCount { get; set; }

		[JsonProperty("common_friend_count")]
		public int CommonFriendCount { get; set; }

		[JsonProperty("common_like_count")]
		public int CommonLikeCount { get; set; }

		[JsonProperty("common_likes")]
		public ObservableCollection<string> CommonLikes { get; set; }

		[JsonProperty("common_friends")]
		public ObservableCollection<string> CommonFriends { get; set; }

		[JsonProperty("created_date")]
		public DateTime CreatedDate { get; set; }

		[JsonProperty("dead")]
		public bool Dead { get; set; }

		[JsonProperty("last_activity_date")]
		public DateTime LastActivityDate { get; set; }

		[JsonProperty("message_count")]
		public int MessageCount { get; set; }

		[JsonProperty("messages")]
		public ObservableCollection<MessageModel> Messages { get; set; }

		[JsonProperty("muted")]
		public bool Muted { get; set; }

		[JsonProperty("participants")]
		public ObservableCollection<string> Participants { get; set; }

		[JsonProperty("pending")]
		public bool Pending { get; set; }


		[JsonProperty("is_super_like")]
		public bool IsSuperLike { get; set; }

		[JsonProperty("following")]
		public bool Following { get; set; }

		[JsonProperty("following_moments")]
		public bool FollowingMoments { get; set; }

		[JsonProperty("spotify_top_artists")]
		public ObservableCollection<SpotifyTopArtistModel> SpotifyTopArtists { get; set; }

		[JsonProperty("spotify_theme_track")]
		public SpotifyTrackModel SpotifyThemeTrack { get; set; }

		[JsonProperty("instagram")]
		public InstagramModel Instagram { get; set; }

		[JsonProperty("person")]
		public PersonModel Person { get; set; }

		[JsonProperty("distance_mi")]
		public int DistanceMiles { get; set; }


		public override string ToString()
		{
			return string.Format($"{Person.Name.Unidecode()}.{Person.Id}");
		}

		public bool ShouldSerializeMessages()
		{
			return false;
		}
	}

	public sealed class MatchUpdateModel
	{
		[JsonProperty("status")]
		public int Status { get; set; }

		[JsonProperty("results")]
		public MatchUpdateResultsModel Results { get; set; }
	}

	public sealed class MatchUpdateResultsModel : MatchModel
	{
		[JsonProperty("bio")]
		public string Bio { get; set; }

		[JsonProperty("birth_date")]
		public DateTime BirthDate { get; set; }

		[JsonProperty("ping_time")]
		public DateTime PingTime { get; set; }

		[JsonProperty("photos")]
		public ObservableCollection<PhotoModel> Photos { get; set; }
	}
}
