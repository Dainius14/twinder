using System;
using RestSharp;
using Newtonsoft.Json;
using Twinder.Model;
using Twinder.Model.Authentication;
using System.Net;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Twinder.Helpers
{
	public static class TinderHelper
	{
		private const string TINDER_API_URL = "https://api.gotinder.com";
		private const string USER_AGENT = "Tinder/4.6.1 (iPhone; iOS 9.0.1; Scale/2.00)";
		private const string APP_VERSION = "371";

		private static RestClient _client;


		/// <summary>
		/// Initializes RestClient with Tinder token
		/// </summary>
		/// <param name="tinderToken">Tinder token</param>
		public static void InitClient(string tinderToken)
		{
			if (_client == null)
			{
				_client = new RestClient(TINDER_API_URL);
				_client.UserAgent = USER_AGENT;
				_client.AddDefaultHeader("app-version", APP_VERSION);
				_client.AddDefaultHeader("X-Auth-Token", tinderToken);
			}
		}

		/// <summary>
		/// Initializes client and authenticates with Tinder server with Facebook ID and Facebook token
		/// </summary>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <returns>True if authentication is successful</returns>
		public static async Task<AuthModel> GetAuthData(string fbId, string fbToken)
		{
			_client = new RestClient(TINDER_API_URL);
			_client.UserAgent = USER_AGENT;
			_client.AddDefaultHeader("app-version", APP_VERSION);

			RestRequest request = new RestRequest("auth", Method.POST);
			request.AddParameter("facebook_id", fbId);
			request.AddParameter("facebook_token", fbToken);

			IRestResponse response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				var auth = await Task.Run(() => JsonConvert.DeserializeObject<AuthModel>(response.Content));
				_client.AddDefaultHeader("X-Auth-Token", auth.Token);
				return auth;
			}
			else
				throw new TinderRequestException("Error getting auth data: " + response.StatusDescription, response);
		}


		/// <summary>
		/// Gets full user data from server, because on authorization it is not full
		/// </summary>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		public static async Task<UserModel> GetFullUserData()
		{
			var request = new RestRequest("profile", Method.POST);
			IRestResponse response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode == HttpStatusCode.OK)
				return await Task.Run(() => JsonConvert.DeserializeObject<UserModel>(response.Content));
			else
				throw new TinderRequestException("Error getting full user data: " + response.StatusDescription, response);

		}

		/// <summary>
		/// Gets updates since date
		/// </summary>
		/// <param name="since">Date from which to get updates, in universal format</param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
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
			else
				throw new TinderRequestException("Error getting updates: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Gets all updates
		/// </summary>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <returns>Returns the update model</returns>
		public static async Task<UpdatesModel> GetUpdates()
		{
			return await GetUpdates(default(DateTime));
		}

		/// <summary>
		/// Gets full data about specified match
		/// </summary>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <param name="matchPersonId">ID of the person</param>
		public static async Task<MatchUpdateResultsModel> GetFullMatchData(string matchPersonId)
		{
			var request = new RestRequest("user/" + matchPersonId, Method.GET);
			request.AddHeader("Content-type", "application/json");

			var response = await _client.ExecuteTaskAsync<dynamic>(request);
			if (response.StatusCode == HttpStatusCode.OK)
				return await Task.Run(() => JsonConvert.DeserializeObject<MatchUpdateModel>(response.Content).Results);
			else
				throw new TinderRequestException("Error getting full match data: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Unmatches with specified match
		/// </summary>
		/// <param name="matchId">ID of the match</param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		public static async void UnmatchPerson(string matchId)
		{
			var request = new RestRequest("user/matches/" + matchId, Method.DELETE);
			request.AddHeader("Content-type", "application/json");

			var response = await _client.ExecuteTaskAsync<dynamic>(request);
			if (response.StatusCode != HttpStatusCode.OK)
				throw new TinderRequestException("Error unmatching: " + response.StatusDescription, response);
		}


		/// <summary>
		/// Sends a message to specified match
		/// </summary>
		/// <param name="matchId">ID of the match</param>
		/// <param name="messageToSend">Message to send</param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <returns>Returns MessageModel if message was sent succesfully.</returns>
		public static async Task<MessageModel> SendMessage(string matchId, string messageToSend)
		{
			var request = new RestRequest("user/matches/" + matchId, Method.POST);
			request.AddHeader("Content-type", "application/json");
			request.AddParameter("message", messageToSend);

			var response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode == HttpStatusCode.OK)
				return await Task.Run(() => JsonConvert.DeserializeObject<MessageModel>(response.Content));
			else
				throw new TinderRequestException("Error sending message: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Likes recommendation
		/// </summary>
		/// <param name="recId">ID of the recommendation</param>
		/// <param name="superLike">True for sending a super like</param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <returns>Returns a MatchModel with match information</returns>
		public static async Task<MatchModel> LikeRecommendation(string recId, bool superLike = false)
		{
			var request = new RestRequest("like/" + recId);
			if (superLike)
			{
				request.Method = Method.POST;
				request.Resource += "/super";
			}
			else
				request.Method = Method.GET;

			var response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode == HttpStatusCode.OK)
				return await Task.Run(() => JsonConvert.DeserializeObject<RecMatchedModel>(response.Content).Match);
			else
				throw new TinderRequestException("Error liking person: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Passes recommendation
		/// </summary>
		/// <param name="recId">ID of the recommendation</param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		public static async Task PassRecommendation(string recId)
		{
			var request = new RestRequest("pass/" + recId, Method.GET);
			var response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new TinderRequestException("Error passing person: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Gets all available recommendations
		/// </summary>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <returns>RecsResultsModel contains either recommendations or an error message</returns>
		public static async Task<RecsResultsModel> GetRecommendations()
		{
			var request = new RestRequest("user/recs", Method.GET);

			var response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode == HttpStatusCode.OK)
				return await Task.Run(() => JsonConvert.DeserializeObject<RecsResultsModel>(response.Content));
			else
				throw new TinderRequestException("Error getting recommendations: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Pings new location
		/// </summary>
		/// <param name="latitude">Latitude</param>
		/// <param name="longtitude">Longtitude</param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		public static async void PingLocation(string latitude, string longtitude)
		{
			latitude = latitude.Replace(',', '.');
			longtitude = longtitude.Replace(',', '.');

			var request = new RestRequest("user/ping", Method.POST);
			request.AddHeader("Content-type", "application/json");
			request.AddJsonBody(new { lat = latitude, lon = longtitude });

			IRestResponse response = await _client.ExecuteTaskAsync<dynamic>(request);

			if (response.StatusCode != HttpStatusCode.OK)
				throw new TinderRequestException("Error pinging location: " + response.StatusDescription, response);
		}

		/// <summary>
		/// Sends updated user info to Tinder
		/// </summary>
		/// <param name="bio"></param>
		/// <param name="minAge"></param>
		/// <param name="maxAge"></param>
		/// <param name="distance">Distance filter in miles</param>
		/// <param name="interestedIn"></param>
		/// <exception cref="TinderRequestException">Bad request data</exception>
		/// <returns>Last active time according to Tinder</returns>
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
			else
				throw new TinderRequestException("Error updating user: " + response.StatusDescription, response);
		}
	}

	[Serializable]
	public class TinderRequestException : Exception
	{
		public IRestResponse Response { get; private set; }

		public TinderRequestException()
		{
		}

		public TinderRequestException(string message) : base(message)
		{
		}

		public TinderRequestException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TinderRequestException(string message, IRestResponse response) : base (message)
		{
			response = Response;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
