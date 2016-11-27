using Newtonsoft.Json;
using Twinder.Models.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Twinder.Models
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
