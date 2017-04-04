using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twinder.Model
{
	public sealed class JobModel
	{
		[JsonProperty("company")]
		[JsonConverter(typeof(JobConverter<CompanyModel>))]
		public CompanyModel Company { get; set; }

		[JsonProperty("title")]
		[JsonConverter(typeof(JobConverter<JobTitleModel>))]
		public JobTitleModel Title { get; set; }

		public override string ToString()
		{
			return Title.Title + " at " + Company.Name;
		}
	}

	public sealed class JobTitleModel
	{
		[JsonProperty("title")]
		public string Title { get; set; }
	}

	public sealed class CompanyModel
	{
		[JsonProperty("name")]
		public string Name { get; set; }

	}
	
	// Tinder sometimes gives a string, or an object containing a string :)
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
