using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace Twinder.Model
{
	public sealed class UpdatesModel
	{
		[JsonProperty("matches")]
		public ObservableCollection<MatchModel> Matches { get; set; }

		[JsonProperty("blocks")]
		public ObservableCollection<string> Blocks { get; set; }

		[JsonProperty("lists")]
		public dynamic Lists{ get; set; }

		[JsonProperty("deleted_lists")]
		public ObservableCollection<string> DeletedLists { get; set; }

		[JsonProperty("liked_messages")]
		public ObservableCollection<LikedMessage> LikedMessages { get; set; }

		[JsonProperty("squads")]
		public dynamic Squads { get; set; }

		[JsonProperty("last_activity_date")]
		public DateTime LastActivityDate { get; set; }
	}
}
