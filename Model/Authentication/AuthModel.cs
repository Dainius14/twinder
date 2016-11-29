using Newtonsoft.Json;

namespace Twinder.Model.Authentication
{
	public sealed class AuthModel
	{
		[JsonProperty("token")]
		public string Token { get; set; }

		[JsonProperty("user")]
		public UserModel User { get; set; }

		[JsonProperty("versions")]
		public Versions Versions { get; set; }

		[JsonProperty("globals")]
		public GlobalsModel Globals { get; set; }
	}
}
