using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinder.Models.Updates;

namespace Twinder.Model
{
	public class RecMatchedModel
	{
		[JsonProperty("match")]
		public MatchModel Match { get; set; }

		[JsonProperty("likes_remaining")]
		public int LikesRemaining { get; set; }
	}
}
