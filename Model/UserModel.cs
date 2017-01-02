using Newtonsoft.Json;
using Twinder.Model.UserRelated;
using System;
using System.Collections.ObjectModel;
using Twinder.Model.Photos;
using Unidecode.NET;

namespace Twinder.Model
{
    public enum Gender
	{
		Man = 0,
		Woman = 1,
		Both = -1
	}

	public sealed class UserModel
	{
		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("api_token")]
		public string ApiToken { get; set; }


		[JsonProperty("active_time")]
		public DateTime ActiveTime { get; set; }

		[JsonProperty("create_date")]
		public DateTime CreateDate { get; set; }

		[JsonProperty("latest_update_date")]
		public DateTime LatestUpdateDate { get; set; }

		[JsonProperty("ping_time")]
		public DateTime PingTime { get; set; }


		[JsonProperty("age_filter_max")]
		public int AgeFilterMax { get; set; }

		[JsonProperty("age_filter_min")]
		public int AgeFilterMin { get; set; }


		[JsonProperty("bio")]
		public string Bio { get; set; }

		[JsonProperty("birth_date")]
		public DateTime BirthDate { get; set; }
		public int Age { get { return (int) (DateTime.Now - BirthDate).TotalDays / 365; } }

		[JsonProperty("connection_count")]
		public int ConnectionCount { get; set; }

		[JsonProperty("friends")]
		public ObservableCollection<string> Friends { get; set; }

		[JsonProperty("distance_filter")]
		public int DistanceFilter { get; set; }

		[JsonProperty("groups")]
		public dynamic Groups { get; set; }

		[JsonProperty("gender")]
		public Gender Gender { get; set; }

		[JsonProperty("gender_filter")]
		public Gender GenderFilter { get; set; }

		[JsonProperty("interested_in")]
		public ObservableCollection<Gender> InterestedIn { get; set; }

		[JsonProperty("interests")]
		public ObservableCollection<InterestModel> Interests { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("full_name")]
		public string FullName { get; set; }


		[JsonProperty("pos")]
		public PositionModel Pos { get; set; }

		[JsonProperty("pos_major")]
		public PositionModel PosMajor { get; set; }


		[JsonProperty("discoverable")]
		public bool Discoverable { get; set; }

		[JsonProperty("photos")]
		public ObservableCollection<PhotoModel> Photos { get; set; }

		[JsonProperty("photos_processing")]
		public bool PhotosProcessing { get; set; }

		[JsonProperty("jobs")]
		public ObservableCollection<JobModel> Jobs { get; set; }

		[JsonProperty("high_school")]
		public ObservableCollection<string> HighSchool { get; set; }

		[JsonProperty("schools")]
		public ObservableCollection<SchoolModel> Schools { get; set; }

		[JsonProperty("can_create_squad")]
		public bool CanCreateSquad { get; set; }

		[JsonProperty("squads_discoverable")]
		public bool SquadsDiscoverable { get; set; }

		[JsonProperty("squads_only")]
		public bool SquadsOnly { get; set; }

		[JsonProperty("squad_ads_shown")]
		public bool SquadAdsShown { get; set; }

		[JsonProperty("promoted_out_of_date")]
		public bool PromotedOutOfDate { get; set; }

		[JsonProperty("purchases")]
		public dynamic Purchases { get; set; }

		[JsonProperty("is_new_user")]
		public bool IsNewUser { get; set; }

		[JsonProperty("blend")]
		public string Blend { get; set; }

		public override string ToString()
		{
			return string.Format($"{Name.Unidecode()}.{Id}");
		}
	}
}
