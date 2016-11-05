using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twinder.Model
{
	public class RecsResultsModel
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("status")]
		public int Status { get; set; }

		[JsonProperty("results")]
		public ObservableCollection<RecommendationModel> Recommendations { get; set; }
	}
}
