using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Twinder.Models.UserRelated;
using Twinder.Models.UserRelated.PhotosRelated;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Twinder.Models
{
	public sealed class UserModel
	{
		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("active_time")]
		public DateTime ActiveTime { get; set; }

		[JsonProperty("can_create_squad")]
		public bool CanCreateSquad { get; set; }

		[JsonProperty("create_date")]
		public DateTime CreateDate { get; set; }

		[JsonProperty("age_filter_max")]
		public int AgeFilterMax { get; set; }

		[JsonProperty("age_filter_min")]
		public int AgeFilterMin { get; set; }

		[JsonProperty("api_token")]
		public string ApiToken { get; set; }

		[JsonProperty("bio")]
		public string Bio { get; set; }

		[JsonProperty("birth_date")]
		public DateTime BirthDate { get; set; }

		[JsonProperty("connection_count")]
		public int ConnectionCount { get; set; }

		[JsonProperty("distance_filter")]
		public int DistanceFilter { get; set; }

		[JsonProperty("full_name")]
		public string FullName { get; set; }

		[JsonProperty("groups")]
		public dynamic Groups { get; set; }

		[JsonProperty("gender")]
		public int Gender { get; set; }

		[JsonProperty("gender_filter")]
		public int GenderFilter { get; set; }

		[JsonProperty("interests")]
		public ObservableCollection<InterestModel> Interests { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("ping_time")]
		public DateTime PingTime { get; set; }

		[JsonProperty("discoverable")]
		public bool Discoverable { get; set; }

		[JsonProperty("photos")]
		public ObservableCollection<PhotoModel> Photos { get; set; }

		[JsonProperty("photos_processing")]
		public bool PhotosProcessing { get; set; }

		[JsonProperty("jobs")]
		public dynamic Jobs { get; set; }

		[JsonProperty("schools")]
		public ObservableCollection<SchoolModel> Schools { get; set; }

		[JsonProperty("squads_discoverable")]
		public bool SquadsDiscoverable { get; set; }

		[JsonProperty("squads_only")]
		public bool SquadsOnly { get; set; }

		[JsonProperty("purchases")]
		public dynamic Purchases { get; set; }

		[JsonProperty("is_new_user")]
		public bool IsNewUser { get; set; }

		public override string ToString()
		{
			return string.Format($"{Name} User");
		}
	}
}
