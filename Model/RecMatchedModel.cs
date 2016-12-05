using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Twinder.Model
{
	public sealed class RecMatchedModel
	{
		[JsonProperty("match")]
		[JsonConverter(typeof(MatchConverter))]
		public MatchModel Match { get; set; }

		[JsonProperty("likes_remaining")]
		public int LikesRemaining { get; set; }
	}

	class MatchConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(bool) || objectType == typeof(MatchModel));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JToken token = JToken.Load(reader);
			// If it's not a match, it gives 'false', so we just change that to null and deal with it later in life
			if (token.Type == JTokenType.Boolean)
			{
				return null;
			}
			if (token.Type == JTokenType.Object)
			{
				return token.ToObject<MatchModel>();
			}
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}
	}

}
