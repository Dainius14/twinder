using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Models.Authentication
{
	public sealed class GlobalsModel
	{
		[JsonProperty("friends")]
		public string Friends { get; set; }

		[JsonProperty("invite_type")]
		public string InviteType { get; set; }

		[JsonProperty("recs_interval")]
		public string RecsInterval { get; set; }

		[JsonProperty("updates_interval")]
		public string UpdatesInterval { get; set; }

		[JsonProperty("recs_size")]
		public string RecsSize { get; set; }

		[JsonProperty("matchmaker_default_message")]
		public string MatchmakerDefaultMessage { get; set; }

		[JsonProperty("share_default_text")]
		public string ShareDefaultText { get; set; }

		[JsonProperty("boost_decay")]
		public string BoostDecay { get; set; }

		[JsonProperty("boost_up")]
		public string BoostUp { get; set; }

		[JsonProperty("boost_down")]
		public string BoostDown { get; set; }

		[JsonProperty("sparks")]
		public string Sparks { get; set; }

		[JsonProperty("kontagent")]
		public string Kontagent { get; set; }

		[JsonProperty("sparks_enabled")]
		public string SparksEnabled { get; set; }

		[JsonProperty("kontagent_enabled")]
		public string KontagentEnabled { get; set; }

		[JsonProperty("mqtt")]
		public string Mqtt { get; set; }

		[JsonProperty("tinder_sparks")]
		public string TinderSparks { get; set; }

		[JsonProperty("moments_interval")]
		public string MomentsInterval { get; set; }

		[JsonProperty("fetch_connections")]
		public string FetchConnections { get; set; }

		[JsonProperty("plus")]
		public string Plus { get; set; }
	}
}
