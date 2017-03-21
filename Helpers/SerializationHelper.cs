using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Twinder.Model;
using Twinder.Model.Photos;
using static Twinder.Properties.Settings;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using BinaryAnalysis.UnidecodeSharp;
using System.Xml.Linq;

namespace Twinder.Helpers
{
	public static class SerializationHelper
	{
		private const string EXT = ".json";
		private const string MSGS = ".msgs";

		public const string USER_FILE = "user.json";

		public const string DIR_MATCHES = "Matches\\";
		public const string DIR_UNMATCHED = "Matches-Unmatched\\";
		public const string DIR_UNMATCHED_BY_ME = "Matches-Unmatched-By-Me\\";

		public const string DIR_RECS = "Recommendations\\";
		public const string DIR_RECS_PENDING = "Recommendations-Pending\\";
		public const string DIR_RECS_PASSED = "Recommendations-Passed\\";

		public const string DIR_USER = "User\\";

		public const string PHOTOS = "Photos\\";
		public const string IG_PHOTOS = "Instagram\\";

		private static WebClient _webClient;
		private static WebClient WebClient
		{
			get
			{
				if (_webClient == null)
				{
					_webClient = new WebClient();
					_webClient.Proxy = new WebProxy();
				}
				return _webClient;
			}
		}

		public static string CurrentUser { get; internal set; }
		public static string WorkingDir { get { return Default.AppDataFolder + CurrentUser + "\\"; } }

		public static void SerializeUser(UserModel user, BackgroundWorker worker)
		{

			if (worker != null)
				worker.ReportProgress(1, 1);

			string workingDir = WorkingDir + DIR_USER;

			DownloadPhotos(user.Photos, workingDir);

			File.WriteAllText(workingDir + USER_FILE,
				JsonConvert.SerializeObject(user, Formatting.Indented));
		}

		/// <summary>
		/// Serializes a given match in it's own folder
		/// </summary>
		/// <param name="match"></param>
		public static void SerializeMatch(MatchModel match)
		{
			string workingDir = WorkingDir + DIR_MATCHES + match.ToString() + "\\";

			// Match doesn't have his own data folder
			if (!Directory.Exists(workingDir))
				CreateMatchFolder(workingDir);


			DownloadPhotos(match.Person.Photos, workingDir);
			DownloadInstagramPhotos(match.Instagram, workingDir);

			// Writes messages
			File.WriteAllText(workingDir + match.Person + MSGS + EXT,
				JsonConvert.SerializeObject(match.Messages, Formatting.Indented));

			// Writes match data
			File.WriteAllText(workingDir + match.Person + EXT,
				JsonConvert.SerializeObject(match, Formatting.Indented));
		}


		/// <summary>
		/// Updates current match model with new data from match update
		/// </summary>
		/// <param name="match"></param>
		/// <param name="matchUpdate"></param>
		public static void UpdateMatchModel(MatchModel match, MatchUpdateResultsModel matchUpdate)
		{
			match.Person.Bio = matchUpdate.Bio;
			match.Person.BirthDate = matchUpdate.BirthDate;

			// Getting full match list update give older ping time
			if (match.Person.PingTime < matchUpdate.PingTime)
				match.Person.PingTime = matchUpdate.PingTime;

			
			var newPhotos = matchUpdate.Photos.Except(match.Person.Photos).ToList();
			var removedPhotos = match.Person.Photos.Except(matchUpdate.Photos).ToList();

			foreach (var item in newPhotos)
				match.Person.Photos.Add(item);
			foreach (var item in removedPhotos)
				match.Person.Photos.Remove(item);

			match.Instagram = matchUpdate.Instagram;
			match.DistanceMiles = matchUpdate.DistanceMiles;
			match.SpotifyThemeTrack = matchUpdate.SpotifyThemeTrack;
			match.SpotifyTopArtists = matchUpdate.SpotifyTopArtists;
			match.CommonFriendCount = matchUpdate.CommonFriendCount;
			match.CommonLikeCount = matchUpdate.CommonLikeCount;
			match.CommonFriends = matchUpdate.CommonFriends;
			match.CommonLikes = matchUpdate.CommonLikes;
			match.ConnectionCount = matchUpdate.ConnectionCount;
		}

		/// <summary>
		/// Updates current match model with data from general updates 
		/// </summary>
		/// <param name="match"></param>
		/// <param name="matchUpdate"></param>
		public static void UpdateMatchModel(MatchModel match, MatchModel matchUpdate)
		{
			match.Person.Bio = matchUpdate.Person.Bio;

			if (match.Person.PingTime < matchUpdate.Person.PingTime)
				match.Person.PingTime = matchUpdate.Person.PingTime;

			if (match.LastActivityDate < matchUpdate.LastActivityDate)
				match.LastActivityDate = matchUpdate.LastActivityDate;

			var newPhotos = matchUpdate.Person.Photos.Except(match.Person.Photos).ToList();
			var removedPhotos = match.Person.Photos.Except(matchUpdate.Person.Photos).ToList();

			foreach (var item in newPhotos)
				match.Person.Photos.Add(item);
			foreach (var item in removedPhotos)
				match.Person.Photos.Remove(item);

			match.Messages.Clear();
			var newMsgs = matchUpdate.Messages.Except(match.Messages);
			foreach (var item in newMsgs)
			{
				match.Messages.Add(item);
			}
		}

		/// <summary>
		/// Serializes a given match in it's own folder
		/// </summary>
		/// <param name="rec"></param>
		public static void SerializeRecommendation(RecModel rec)
		{
			string workingDir = WorkingDir + DIR_RECS + rec.ToString() + "\\";

			// Rec doesn't have his own data folder
			if (!Directory.Exists(workingDir))
				CreateMatchFolder(workingDir);

			DownloadPhotos(rec.Photos, workingDir);
			DownloadInstagramPhotos(rec.Instagram, workingDir);

			File.WriteAllText(workingDir + rec.Name.Unidecode() + EXT,
				JsonConvert.SerializeObject(rec, Formatting.Indented));
		}

		/// <summary>
		/// Serializes all matches
		/// </summary>
		/// <param name="matchList"></param>
		public static void SerializeMatchList(ObservableCollection<MatchModel> matchList, BackgroundWorker worker,
			bool fullMatchData = false)
		{
			for (int i = 0; i < matchList.Count; i++)
			{

				if (fullMatchData)
				{
					// This needs something better but I don't know how
					Task<MatchUpdateResultsModel> t = TinderHelper.GetFullMatchData(matchList[i].Person.Id);
					var updatedMatch = t.Result;
					UpdateMatchModel(matchList[i], updatedMatch);
				}

				if (worker != null)
					worker.ReportProgress(1, i + 1);

				SerializeMatch(matchList[i]);
			}

		}


		/// <summary>
		/// Serializes all recommendations
		/// </summary>
		/// <param name="recList"></param>
		public static void SerializeRecList(ObservableCollection<RecModel> recList, BackgroundWorker worker)
		{
			for (int i = 0; i < recList.Count; i++)
			{
				if (worker != null)
					worker.ReportProgress(1, i + 1);

				SerializeRecommendation(recList[i]);
			}
		}

		/// <summary>
		/// Moves recommendation to "Pending recommendations" directory
		/// </summary>
		/// <param name="rec"></param>
		private static bool MoveRec(RecModel rec, string dir)
		{
			if (IsDirInAppData(WorkingDir))
			{
				var fromDir = WorkingDir + DIR_RECS + rec;
				var toDir = WorkingDir + dir + rec;

				try
				{
					if (Directory.Exists(toDir))
						Directory.Delete(fromDir, true);
					else
						Directory.Move(fromDir, toDir);
					return true;
				}
				catch (UnauthorizedAccessException e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
				catch (IOException e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
			}
			return false;
		}

		public static bool MoveRecToPassed(RecModel rec)
		{
			return MoveRec(rec, DIR_RECS_PASSED);
		}
		public static bool MoveRecToPending(RecModel rec)
		{
			return MoveRec(rec, DIR_RECS_PENDING);
		}

		public static bool MoveRecToMatches(RecModel rec)
		{
			return MoveRecPhotosToMatches(rec.ToString(), DIR_RECS);
		}

		/// <summary>
		/// Moves photos of recommendation from "Pending recommendations" to "Matches" directory
		/// </summary>
		/// <param name="rec"></param>
		public static bool MoveRecFromPendingToMatches(RecModel rec)
		{
			return MoveRecPhotosToMatches(rec.ToString(), DIR_RECS_PENDING);
		}

		/// <summary>
		/// Moves photos of given recommendation to matches directory
		/// </summary>
		/// <param name="recName"></param>
		/// <param name="dir">What dir to move from</param>
		/// <returns></returns>
		private static bool MoveRecPhotosToMatches(string recName, string dir)
		{
			if (IsDirInAppData(WorkingDir))
			{
				var fromRecDir = WorkingDir + dir + recName + "\\";
				var fromDirPhotos = fromRecDir + PHOTOS;
				var fromDirIG = fromRecDir + IG_PHOTOS;

				var toRecDir = WorkingDir + DIR_MATCHES + recName + "\\";
				var toDirPhotos = toRecDir + PHOTOS;
				var toDirIG = toRecDir + IG_PHOTOS;
			
				Directory.CreateDirectory(toRecDir);

				try
				{
					if (Directory.Exists(fromDirPhotos) && !Directory.Exists(toDirPhotos))
						Directory.Move(fromDirPhotos, toDirPhotos);

					if (Directory.Exists(fromDirIG) && !Directory.Exists(toDirIG))
						Directory.Move(fromDirIG, toDirIG);

					Directory.Delete(fromRecDir, true);
					return true;
				}
				catch (UnauthorizedAccessException e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
				catch (IOException e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
			}
			return false;
		}

		/// <summary>
		/// Moves match to "Unmatched by me" dir
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public static bool MoveMatchToUnMatchedByMe(MatchModel match)
		{
			return MoveToUnmatched(match.ToString(), DIR_UNMATCHED_BY_ME);
		}

		/// <summary>
		/// Moves match to "Unmatched" dir
		/// </summary>
		/// <param name="match"></param>
		/// <returns></returns>
		public static bool MoveMatchToUnMatched(MatchModel match)
		{
			return MoveToUnmatched(match.ToString(), DIR_UNMATCHED);
		}

		/// <summary>
		/// Moves match to the specified directory
		/// </summary>
		/// <param name="matchName"></param>
		/// <param name="dir">Directory to move to</param>
		/// <returns></returns>
		private static bool MoveToUnmatched(string matchName, string dir)
		{
			if (IsDirInAppData(WorkingDir))
			{
				var fromDir = WorkingDir + DIR_MATCHES + matchName + "\\";
				var toDir = WorkingDir + dir + matchName + "\\";
				try
				{
					if (Directory.Exists(toDir))
						Directory.Delete(fromDir, true);
					else
						Directory.Move(fromDir, toDir);
					return true;
				}
				catch (UnauthorizedAccessException e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
				catch (IOException e)
				{
					MessageBox.Show("Error: " + e.ToString());
				}
			}
			return false;
		}
		
		/// <summary>
		/// Deserializes given item - <see cref="MatchModel"/>, <see cref="RecModel"/> or <see cref="UserModel"/>
		/// </summary>
		/// <param name="folderName"></param>
		/// <returns></returns>
		private static T DeserializeItem<T>(string folderName) where T : ISerializableItem
		{
			var itemName = folderName.Substring(folderName.LastIndexOf("\\") + 1);
			itemName = itemName.Remove(itemName.LastIndexOf('.'));
			itemName = itemName.Unidecode();

			var fullPath = folderName + "\\" + itemName + EXT;

			if (File.Exists(fullPath))
			{
				var item = JsonConvert.DeserializeObject<T>(File.ReadAllText(fullPath));
				// If it is a match, don't forget messages
				
				if (item != null)
				{
					if (typeof(T) == typeof(MatchModel))
						(item as MatchModel).Messages = 
							JsonConvert.DeserializeObject<ObservableCollection<MessageModel>>(
							File.ReadAllText(folderName + "\\" + itemName + MSGS + EXT))
							?? new ObservableCollection<MessageModel>();
					return item;
				}
				
			}
			// We fucked up somewhere - rollback
			fullPath = fullPath.Remove(fullPath.LastIndexOf(itemName));
			if (IsDirInAppData(fullPath))
				Directory.Delete(fullPath, true);
			return default(T);

		}

		/// <summary>
		/// Deserializes all of the items in given directory
		/// </summary>
		/// <returns></returns>
		private static ObservableCollection<T> DeserializeListFolder<T>(string dir) where T : ISerializableItem
		{
			var list = new ObservableCollection<T>();
			var dirs = Directory.GetDirectories(WorkingDir + dir);

			foreach (var item in dirs)
			{
				var itemToAdd = DeserializeItem<T>(item);
				if (itemToAdd != null && !itemToAdd.Equals(default(T)))
					list.Add(itemToAdd);
			}
			return list;
		}

		public static ObservableCollection<MatchModel> DeserializeMatchList()
		{
			return DeserializeListFolder<MatchModel>(DIR_MATCHES);
		}

		public static ObservableCollection<MatchModel> DeserializeUnmatchedList()
		{
			return DeserializeListFolder<MatchModel>(DIR_UNMATCHED);
		}

		public static ObservableCollection<MatchModel> DeserializeUnmatchedByMeList()
		{
			return DeserializeListFolder<MatchModel>(DIR_UNMATCHED_BY_ME);
		}


		public static ObservableCollection<RecModel> DeserializeRecList()
		{
			return DeserializeListFolder<RecModel>(DIR_RECS);
		}

		public static ObservableCollection<RecModel> DeserializeRecPassedList()
		{
			return DeserializeListFolder<RecModel>(DIR_RECS_PASSED);
		}

		public static ObservableCollection<RecModel> DeserializeRecPendingList()
		{
			return DeserializeListFolder<RecModel>(DIR_RECS_PENDING);
		}

		/// <summary>
		/// Deletes all recs from "Recommendations" folder
		/// </summary>
		public static bool EmptyRecommendations()
		{
			if (IsDirInAppData(WorkingDir))
			{
				var dirs = Directory.EnumerateDirectories(WorkingDir + DIR_RECS).ToList();
				if (dirs.Count != 0)
				{
					foreach (var dir in dirs)
						Directory.Delete(dir, true);
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Saves person's photos to specified directory
		/// </summary>
		/// <param name="photos"></param>
		/// <param name="dir"></param>
		private static void DownloadPhotos(ObservableCollection<PhotoModel> photos, string dir)
		{
			if (photos != null)
			{
				string picDir = dir + PHOTOS;

				Parallel.ForEach(photos, photo =>
				{
					string downloadPath = picDir + (photo.FileName ?? photo.Id + ".jpg");

					if (!File.Exists(downloadPath))
						// Get the 640px photo
						foreach (var processedPic in photo.ProcessedFiles)
							if (processedPic.Height == 640)
							{
								try
								{
									new WebClient().DownloadFile(new Uri(processedPic.Url), downloadPath);
								}
								catch
								{
									// sometimes 403 is returned
								}
								break;
							}

				});
			}
		}

		/// <summary>
		/// Saves Instagram photos to specified directory
		/// </summary>
		/// <param name="instagram"></param>
		/// <param name="dir"></param>
		private static void DownloadInstagramPhotos(InstagramModel instagram, string dir)
		{
			if (instagram != null)
			{
				Directory.CreateDirectory(dir + IG_PHOTOS);

				string instaDir = dir + IG_PHOTOS;
				Parallel.ForEach(instagram.InstagramPhotos, photo =>
				{
					string fileName = instaDir + photo.Link.Substring(28, photo.Link.Length - 28 - 2) + ".jpg";

					// Downloads only if the file does not yet exist
					if (!File.Exists(instaDir + fileName))
					{
						try
						{
							if (!File.Exists(fileName))
								new WebClient().DownloadFile(new Uri(photo.Image), fileName);
						}
						catch
						{
							// In case we also get 403 here, we just skip those pics who cares
						}
					}
				});
			}
		}

		/// <summary>
		/// Creates user folder and config file
		/// </summary>
		/// <param name="fbId"></param>
		/// <param name="fbToken"></param>
		/// <param name="userName"></param>
		public static void CreateUser(string fbId, string fbToken, string userName, DateTime lastUpdate,
			string lat, string lon)
		{
			CurrentUser = userName;

			Directory.CreateDirectory(WorkingDir);

			Directory.CreateDirectory(WorkingDir + DIR_MATCHES);
			Directory.CreateDirectory(WorkingDir + DIR_UNMATCHED);
			Directory.CreateDirectory(WorkingDir + DIR_UNMATCHED_BY_ME);

			Directory.CreateDirectory(WorkingDir + DIR_RECS);
			Directory.CreateDirectory(WorkingDir + DIR_RECS_PENDING);
			Directory.CreateDirectory(WorkingDir + DIR_RECS_PASSED);

			Directory.CreateDirectory(WorkingDir + DIR_USER);
			Directory.CreateDirectory(WorkingDir + DIR_USER + PHOTOS);
			

			// Creates XML file with data
			var doc = new XDocument(
				new XElement("LoginData",
					new XElement("FbId", fbId),
					new XElement("FbToken", fbToken),
					new XElement("TinderToken", fbToken),
					new XElement("LastUpdate", lastUpdate),
					new XElement("Latitude", string.Empty),
					new XElement("Longtitude", string.Empty),
					new XElement("SuperLikeResetAt", default(DateTime)),
					new XElement("SuperLikesLeft", 0),
					new XElement("MaxSuperLikes", 1)));

			doc.Save(WorkingDir + "\\config.xml");
		}
		
		public static void UpdateUserPosition(string lat, string lon)
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);
				doc.Element("LoginData").Element("Latitude").SetValue(lat);
				doc.Element("LoginData").Element("Longtitude").SetValue(lon);
				doc.Save(file);
			}
		}

		public static void UpdateTinderToken(string token)
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);
				doc.Element("LoginData").Element("TinderToken").SetValue(token);
				doc.Save(file);
			}
		}

		public static void UpdateLastUpdate(DateTime lastUpdate)
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);
				doc.Element("LoginData").Element("LastUpdate").SetValue(lastUpdate);
				doc.Save(file);
			}
		}

		public static void UpdateSuperLikes(DateTime resetAt = default(DateTime), int likesLeft = -1, int max = -1)
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);

				var elReset = doc.Element("LoginData").Element("SuperLikeResetAt");
				var elLeft = doc.Element("LoginData").Element("SuperLikesLeft");
				var elMax = doc.Element("LoginData").Element("MaxSuperLikes");

				if (resetAt != default(DateTime))
				{
					if (elReset != null)
						elReset.SetValue(resetAt);
					else
						doc.Element("LoginData").Add(new XElement("SuperLikeResetAt", resetAt));
				}

				if (likesLeft != -1)
				{
					if (elLeft != null)
						elLeft.SetValue(likesLeft);
					else
						doc.Element("LoginData").Add(new XElement("SuperLikesLeft", likesLeft));
				}

				if (max != -1)
				{
					if (elMax != null)
						elMax.SetValue(1);
					else
						doc.Element("LoginData").Add(new XElement("MaxSuperLikes", max));
				}

				doc.Save(file);
			}
		}

		public static DateTime GetSuperLikeReset()
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);

				var el = doc.Element("LoginData").Element("SuperLikeResetAt");
				if (el != null)
					return Convert.ToDateTime(el.Value);
				doc.Element("LoginData").Add(new XElement("SuperLikeResetAt", default(DateTime)));
				doc.Save(file);
			}
			return default(DateTime);
		}

		public static int GetSuperLikesLeft()
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);

				var el = doc.Element("LoginData").Element("SuperLikesLeft");
				if (el != null)
					return int.Parse(el.Value);
				doc.Element("LoginData").Add(new XElement("SuperLikesLeft", default(int)));
				doc.Save(file);
			}
			return default(int);
		}

		public static int GetMaxSuperLikes()
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);

				var el = doc.Element("LoginData").Element("MaxSuperLikes");
				if (el != null)
					return int.Parse(el.Value);
				doc.Element("LoginData").Add(new XElement("MaxSuperLikes", 1));
				doc.Save(file);
			}
			return default(int);
		}


		public static DateTime GetLastUpdate()
		{
			var file = WorkingDir + "config.xml";
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);
				return Convert.ToDateTime((doc.Element("LoginData").Element("LastUpdate").Value));
			}
			return default(DateTime);
		}

		/// <summary>
		/// Returns saved position through parameters, because too lazy
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		public static void GetUserPosition(out string lat, out string lon)
		{
			var file = WorkingDir + "config.xml";
			lat = string.Empty;
			lon = string.Empty;
			if (File.Exists(file))
			{
				var doc = XDocument.Load(file);
				lat = doc.Element("LoginData").Element("Latitude").Value;
				lon = doc.Element("LoginData").Element("Longtitude").Value;
			}
		}

		/// <summary>
		/// Creates folders for match data
		/// </summary>
		/// <param name="root"></param>
		private static void CreateMatchFolder(string root)
		{
			Directory.CreateDirectory(root);
			Directory.CreateDirectory(root + PHOTOS);
		}

		private static bool IsDirInAppData(string dir)
		{
			return dir.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Twinder");
		}
		
	}

	/// <summary>
	/// For use when sending from ViewModel multiple values to serialize
	/// </summary>
	public sealed class SerializationPacket
	{
		public bool FullMatchData { get; set; }
		public ObservableCollection<MatchModel> MatchList { get; set; }
		public ObservableCollection<RecModel> RecList { get; set; }
		public UserModel User { get; set; }

		public SerializationPacket(ObservableCollection<MatchModel> matchList, bool fullMatchData = false)
		{
			MatchList = matchList;
			FullMatchData = fullMatchData;
		}

		public SerializationPacket(ObservableCollection<RecModel> recList)
		{
			RecList = recList;
		}

		public SerializationPacket(UserModel user)
		{
			User = user;
		}

		public SerializationPacket(ObservableCollection<MatchModel> matchList,
			ObservableCollection<RecModel> recList, UserModel user)
		{
			MatchList = matchList;
			RecList = recList;
			User = user;
		}
	}

	public class DirPath
	{
		public static string DIR_MATCHES { get { return "Matches\\"; } }
		public static string DIR_UNMATCHED { get { return "Matches-Unmatched\\"; } }
		public static string DIR_UNMATCHED_BY_ME { get { return "Matches-Unmatched-By-Me\\"; } }
		public static string DIR_RECS { get { return "Recommendations\\"; } }
		public static string DIR_RECS_PENDING { get { return "Recommendations-Pending\\"; } }
		public static string DIR_RECS_PASSED { get { return "Recommendations-Passed\\"; } }
	}
}
