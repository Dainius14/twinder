using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Twinder.Model
{
	public sealed class LikeSentModel
	{
		[JsonProperty("match")]
		[JsonConverter(typeof(MatchedConverter))]
		public MatchModel Match { get; set; }

		[JsonProperty("status")]
		public int Status { get; set; }

		/// <summary>
		/// Simple like was sent
		/// </summary>
		[JsonProperty("likes_remaining")]
		public int LikesRemainig { get; set; }

		/// <summary>
		/// Super like was sent
		/// </summary>
		[JsonProperty("super_likes")]
		public SuperLikesModel SuperLikes { get; set; }
	}

	public sealed class SuperLikesModel
	{
		[JsonProperty("remaining")]
		public int Remaining { get; set; }

		[JsonProperty("alc_remaining")]
		public int AlcRemaining { get; set; }

		[JsonProperty("new_alc_remaining")]
		public int NewAlcRemaining { get; set; }

		[JsonProperty("allotment")]
		public int Allotment { get; set; }

		[JsonProperty("superlike_refresh_amount")]
		public int SuperLikeRefreshAmount { get; set; }

		[JsonProperty("superlike_refresh_interval")]
		public int SuperLikeRefreshInteval { get; set; }

		[JsonProperty("superlike_refresh_interval_unit")]
		public string SuperLikeRefreshIntervalUnit { get; set; }

		[JsonProperty("resets_at")]
		public DateTime ResetsAt { get; set; }
	}
	
	/// <summary>
	/// Bool is returned when there's no match, <see cref="MatchModel"/> is returned when there is
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class MatchedConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(bool) || objectType == typeof(MatchModel));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JToken token = JToken.Load(reader);
			// If it's not a match, it gives 'false', so basically it's a null
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
