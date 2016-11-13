using Newtonsoft.Json;

namespace Twinder.Model
{
	public class CompanyModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

	}
}
