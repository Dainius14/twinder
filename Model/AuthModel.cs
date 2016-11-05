using Newtonsoft.Json;
using Twinder.Models.Authentication;

namespace Twinder.Models.Authentication
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
