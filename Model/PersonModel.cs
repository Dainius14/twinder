using BinaryAnalysis.UnidecodeSharp;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using Twinder.Helpers;
using Twinder.Model.Photos;

namespace Twinder.Model
{
	public sealed class PersonModel
	{
		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("bio")]
		public string Bio { get; set; }

		[JsonProperty("birth_date")]
		public DateTime BirthDate { get; set; }
		public int Age { get { return (int)(DateTime.Now - BirthDate).TotalDays / 365 - 1; } }

		[JsonProperty("gender")]
		public int Gender { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("ping_time")]
		public DateTime PingTime { get; set; }

		[JsonProperty("photos")]
		public ObservableCollection<PhotoModel> Photos { get; set; }

		//[JsonProperty("badges")]
		public dynamic Badges{ get; set; }

		public override string ToString()
		{
			return string.Format($"{Name.Unidecode()}");
		}
	}
}