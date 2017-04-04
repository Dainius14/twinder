using Newtonsoft.Json;

namespace Twinder.Model.UserRelated
{
	public sealed class SchoolModel
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("year")]
		public string Year { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("displayed")]
		public bool Displayed { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
