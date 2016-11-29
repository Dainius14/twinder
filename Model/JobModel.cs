using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twinder.Model.Auth.User
{
	public class JobModel
	{
		[JsonProperty("company")]
		[JsonConverter(typeof(JobConverter<CompanyModel>))]
		public CompanyModel Company { get; set; }

		[JsonProperty("title")]
		[JsonConverter(typeof(JobConverter<JobTitle>))]
		public JobTitle Title { get; set; }
	}

	public class JobTitle
	{
		[JsonProperty("title")]
		public string Title { get; set; }
	}

	// TODO fix this mess
	class JobConverter<T> : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(T) || objectType == typeof(string));
		}
		
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JToken token = JToken.Load(reader);
			if (token.Type == JTokenType.String)
			{
				return token.ToObject<string>();
			}
			if (token.Type == JTokenType.Object)
			{
				return token.ToObject<T>();
			}
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value);
		}
	}
}
