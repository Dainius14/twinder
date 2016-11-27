using System;
using RestSharp;
using Newtonsoft.Json;
using Twinder.Properties;
using Twinder.Models;
using Twinder.Models.Authentication;
using Twinder.Models.Updates;
using Twinder.Model;
using System.Net;
using System.Threading.Tasks;
using Twinder.ViewModel;

namespace Twinder.Helpers
{
	public static class TinderHelper
	{
		private const string TINDER_API_URL = "https://api.gotinder.com";
		private const string USER_AGENT = "Tinder/4.6.1 (iPhone; iOS 9.0.1; Scale/2.00)";
		private const string APP_VERSION = "371";

		private static string _fbToken, _fbId, _tinderToken;
		private static RestClient _client;
		
		public static AuthModel Auth { get; private set; }
		public static UserModel User { get; private set; }
		public static UpdatesModel Updates { get; private set; }


		/// <summary>
		/// Authenticates with Tinder server with Facebook ID and Facebook Token
		/// </summary>
		/// <returns>True if authentication is successful</returns>
		public static async Task<bool> Authenticate()
		{
			_fbId = Settings.Default.fb_id;
			_fbToken = Settings.Default.fb_token;

			_client = new RestClient(TINDER_API_URL);
			_client.UserAgent = USER_AGENT;
			_client.AddDefaultHeader("app-version", APP_VERSION);

			RestRequest request = new RestRequest("auth", Method.POST);
			request.AddParameter("facebook_id", _fbId);
			request.AddParameter("facebook_token", _fbToken);

			IRestResponse response = await _client.ExecuteTaskAsync<dynamic>(request);
			
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Auth = await Task.Run(() => JsonConvert.DeserializeObject<AuthModel>(response.Content));
				_tinderToken = Auth.Token;
				_client.AddDefaultHeader("X-Auth-Token", _tinderToken);
				
				// Sends second request to get all user data
				request = new RestRequest("profile", Method.POST);
				response = await _client.ExecuteTaskAsync<dynamic>(request);
				User = await Task.Run(() => JsonConvert.DeserializeObject<UserModel>(response.Content));
				
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets updates since date
		/// </summary>
		/// <param name="since">Date from which to get updates</param>
		/// <returns>Returns the Update model</returns>
		public static async Task<UpdatesModel> GetUpdates(DateTime since)
		{
			RestRequest request = new RestRequest("updates", Method.POST);

			if (since != default(DateTime))
				request.AddParameter("last_activity_date", since.ToString("o"));

			IRestResponse response = await _client.ExecuteTaskAsync<dynamic>(request);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await Task.Run(() => JsonConvert.DeserializeObject<UpdatesModel>(response.Content));
			}
			return null;
		}

		/// <summary>
		/// Gets all updates
		/// </summary>
		/// <returns>Returns the update model</returns>
		public static async Task<UpdatesModel> GetUpdates()
		{
			return await GetUpdates(default(DateTime));
		}

		/// <summary>
		/// Sends a message to specified match
		/// </summary>
		/// <param name="matchId">Match to whom to send message</param>
		/// <param name="messageToSend">Message to send</param>
		/// <returns>Returns MessageModel if message was sent succesfully.</returns>
		public static async Task<MessageModel> SendMessage(string matchId, string messageToSend)
		{
			RestRequest request = new RestRequest("user/matches/" + matchId, Method.POST);
			request.AddHeader("Content-type", "application/json");
			request.AddParameter("message", messageToSend);

			var response = await _client.ExecuteTaskAsync<dynamic>(request);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await Task.Run(() => JsonConvert.DeserializeObject<MessageModel>(response.Content));
			}
			return null;
		}

		/// <summary>
		/// Likes recommendation
		/// </summary>
		/// <param name="id">Recommendation ID</param>
		/// <param name="superLike">True for making a super like</param>
		/// <returns>Returns a MatchModel with match information</returns>
		public static async Task<MatchModel> LikeRecommendation(string id, bool superLike = false)
		{
			var request = new RestRequest("like/" + id);
			if (superLike)
			{
				request.Method = Method.POST;
				request.Resource += "/super";
			}
			else
				request.Method = Method.GET;

			var response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await Task.Run(() => JsonConvert.DeserializeObject<RecMatchedModel>(response.Content).Match);
			}
			return null;
		}

		/// <summary>
		/// Passes recommendation
		/// </summary>
		/// <param name="id">Recommendation ID</param>
		public static async void PassRecommendation(string id)
		{
			var request = new RestRequest("pass/" + id, Method.GET);
			var response = await _client.ExecuteTaskAsync<dynamic>(request);
		}

		/// <summary>
		/// Gets all available recommendations
		/// </summary>
		/// <returns>RecsResultsModel contains either recommendations or an error message</returns>
		public static async Task<RecsResultsModel> GetRecommendations()
		{
			var request = new RestRequest("user/recs", Method.GET);

			var response = await _client.ExecuteTaskAsync<dynamic>(request);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return await Task.Run(() => JsonConvert.DeserializeObject<RecsResultsModel>(response.Content));
			}
			return null;
		}

		/// <summary>
		/// Pings new location
		/// </summary>
		/// <param name="latitude">Latitude</param>
		/// <param name="longtitude">Longtitude</param>
		public static async void PingLocation(string latitude, string longtitude)
		{
			latitude = latitude.Replace(',', '.');
			longtitude = longtitude.Replace(',', '.');

			var request = new RestRequest("user/ping", Method.POST);
			request.AddHeader("Content-type", "application/json");
			request.AddJsonBody(new { lat = latitude, lon = longtitude });

			var response = await _client.ExecuteTaskAsync<dynamic>(request);
			var deserialized = await Task.Run(() => JsonConvert.DeserializeObject<dynamic>(response.Content));
		}

		public static async Task<DateTime> UpdateUser(string bio, int minAge, int maxAge, int distance, Gender interestedIn)
		{

			var request = new RestRequest("profile", Method.POST);
			request.AddHeader("Content-type", "application/json");
			request.AddJsonBody(new
			{
				bio = bio,
				age_filter_min = minAge,
				age_filter_max = maxAge,
				distance = distance,
				gender_filter = (int) interestedIn
			});

			var response = await _client.ExecuteTaskAsync<dynamic>(request);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				var deserialized = await Task.Run(() => JsonConvert.DeserializeObject<UserModel>(response.Content));
				return deserialized.ActiveTime;
			}
			return default(DateTime);
		}
	}
}
