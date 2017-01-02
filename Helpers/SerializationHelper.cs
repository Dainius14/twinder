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
using Unidecode.NET;

namespace Twinder.Helpers
{
    public static class SerializationHelper
	{
		private const string EXT = ".json";
		private const string MSGS = ".msgs";

		private const string DIR_MATCHES = "Matches\\";
		private const string DIR_UNMATCHED = "Matches-Unmatched\\";
		private const string DIR_UNMATCHED_BY_ME = "Matches-Unmatched-By-Me\\";

		private const string DIR_RECS = "Recommendations\\";
		private const string DIR_RECS_PENDING = "Recommendations-Pending\\";
		private const string DIR_RECS_PASSED = "Recommendations-Passed\\";

		private const string DIR_USER = "User\\";

		private const string PHOTOS = "Photos\\";
		private const string IG_PICS = "Instagram\\";
		private const string IG_THUMBS = "Thumbnails\\";

		private const string PIC_PX = "px\\";
		private const string PIC_84PX = "84px\\";
		private const string PIC_172PX = "172px\\";
		private const string PIC_320PX = "320px\\";

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

		public static void SerializeUser(UserModel user, BackgroundWorker worker)
		{

			if (worker != null)
				worker.ReportProgress(1, 1);

			string workingDir = Default.AppDataFolder + DIR_USER;

			DownloadPhotos(user.Photos, workingDir);

			File.WriteAllText(workingDir + user.ToString() + EXT,
				JsonConvert.SerializeObject(user, Formatting.Indented));
		}

		/// <summary>
		/// Serializes a given match in it's own folder
		/// </summary>
		/// <param name="match"></param>
		public static void SerializeMatch(MatchModel match)
		{
			string workingDir = Default.AppDataFolder + DIR_MATCHES + match.ToString() + "\\";

			// Match doesn't have his own data folder
			if (!Directory.Exists(workingDir))
				CreateMatchFolder(workingDir);


			DownloadPhotos(match.Person.Photos, workingDir);
			DownloadInstagramPhotos(match.Instagram, workingDir);

			// After downloading all photos writes back local URLs again
			DeserializePhotos(match.Person.Photos, workingDir);

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

			
			var newPhotos = matchUpdate.Photos.Except(match.Person.Photos);
			var removedPhotos = match.Person.Photos.Except(matchUpdate.Photos);

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
			
			var newPhotos = matchUpdate.Person.Photos.Except(match.Person.Photos);
			var removedPhotos = match.Person.Photos.Except(matchUpdate.Person.Photos);

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
			string workingDir = Default.AppDataFolder + DIR_RECS + rec.ToString() + "\\";

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
		public static bool MoveRecToPending(RecModel rec)
		{
			var fromDir = Default.AppDataFolder + DIR_RECS + rec;
			var toDir = Default.AppDataFolder + DIR_RECS_PENDING + rec;

			try
			{
				Directory.Move(fromDir, toDir);
				return true;
			}
			catch (UnauthorizedAccessException e)
			{
				MessageBox.Show("Error: " + e.ToString());
				return false;
			}
			catch (IOException e)
			{
				MessageBox.Show("Error: " + e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Moves recommendation to "Passed recommendations" directory
		/// </summary>
		/// <param name="rec"></param>
		public static bool MoveRecToPassed(RecModel rec)
		{
			var fromDir = Default.AppDataFolder + DIR_RECS + rec;
			var toDir = Default.AppDataFolder + DIR_RECS_PASSED + rec;

			try
			{
				Directory.Move(fromDir, toDir);
				return true;
			}
			catch (UnauthorizedAccessException e)
			{
				MessageBox.Show("Error: " + e.ToString());
				return false;
			}
			catch (IOException e)
			{
				MessageBox.Show("Error: " + e.ToString());
				return false;
			}
		}

		/// <summary>
		/// Moves photos of recommendation from "Recommendations" to "Matches" directory
		/// </summary>
		/// <param name="rec"></param>
		/// <returns></returns>
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
			var fromRecDir = Default.AppDataFolder + dir + recName + "\\";
			var fromDirPhotos = fromRecDir + PHOTOS;
			var fromDirIG = fromRecDir + IG_PICS;

			var toRecDir = Default.AppDataFolder + DIR_MATCHES + recName + "\\";
			var toDirPhotos = toRecDir + PHOTOS;
			var toDirIG = toRecDir + IG_PICS;

			Directory.CreateDirectory(toRecDir);

			try
			{
				if (Directory.Exists(fromDirPhotos))
					Directory.Move(fromDirPhotos, toDirPhotos);

				if (Directory.Exists(fromDirIG))
					Directory.Move(fromDirIG, toDirIG);

				Directory.Delete(fromRecDir, true);
				return true;
			}
			catch (UnauthorizedAccessException e)
			{
				MessageBox.Show("Error: " + e.ToString());
				return false;
			}
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
			var fromDir = Default.AppDataFolder + DIR_MATCHES + matchName + "\\";
			var toDir = Default.AppDataFolder + dir + matchName + "\\";
			try
			{
				Directory.Move(fromDir, toDir);
				return true;
			}
			catch (UnauthorizedAccessException e)
			{
				MessageBox.Show("Error: " + e.ToString());
				return false;
			}

		}


		/// <summary>
		/// Deserializes given match
		/// </summary>
		/// <param name="folderName"></param>
		/// <returns></returns>
		public static MatchModel DeserializeMatch(string folderName)
		{
			var matchName = folderName.Substring(folderName.LastIndexOf("\\") + 1);
			matchName = matchName.Remove(matchName.LastIndexOf('.'));
			matchName = matchName.Unidecode();

			var match = JsonConvert.DeserializeObject<MatchModel>(File.ReadAllText(folderName + "\\" + matchName + EXT));
			match.Messages = JsonConvert.DeserializeObject<ObservableCollection<MessageModel>>(
				File.ReadAllText(folderName + "\\" + matchName + MSGS + EXT)) ?? new ObservableCollection<MessageModel>();

			//DeserializePhotos(match.Person.Photos, folderName);


			return match;
		}
		

		/// <summary>
		/// Deserializes given recommendation
		/// </summary>
		/// <param name="folderName"></param>
		/// <returns></returns>
		public static RecModel DeserializeRecommendation(string folderName)
		{
			var recName = folderName.Substring(folderName.LastIndexOf("\\") + 1);
			recName = recName.Remove(recName.LastIndexOf('.'));
			recName = recName.Unidecode();

			var rec = JsonConvert.DeserializeObject<RecModel>(File.ReadAllText(folderName + "\\" + recName + EXT));
			return rec;
		}

		/// <summary>
		/// Deserializes all of the recs in Recommendations directory
		/// </summary>
		/// <returns></returns>
		public static ObservableCollection<RecModel> DeserializeRecList()
		{
			var recList = new ObservableCollection<RecModel>();
			var dirs = Directory.GetDirectories(Default.AppDataFolder + DIR_RECS);

			foreach (var item in dirs)
			{
				recList.Add(DeserializeRecommendation(item));
			}
			return recList;
		}


		/// <summary>
		/// Deserializes all of the matches in Matches directory
		/// </summary>
		/// <returns></returns>
		public static ObservableCollection<MatchModel> DeserializeMatchList()
		{
			var matchList = new ObservableCollection<MatchModel>();
			var dirs = Directory.GetDirectories(Default.AppDataFolder + DIR_MATCHES);

			foreach (var item in dirs)
			{
				matchList.Add(DeserializeMatch(item));
			}
			return matchList;
		}

		/// <summary>
		/// Replaces internet URLs by local URLs in storage
		/// </summary>
		/// <param name="photos"></param>
		/// <param name="folderName"></param>
		public static void DeserializePhotos(ObservableCollection<PhotoModel> photos, string folderName)
		{
			for (int i = 0; i < photos.Count; i++)
			{
				string fileName = photos[i].FileName;
				folderName += "\\" + PHOTOS;

				if (File.Exists(folderName + fileName))
					for (int j = 0; j < photos[i].ProcessedFiles.Count; j++)
					{
						var processedFile = photos[i].ProcessedFiles[j];
						if (processedFile.Height == 640)
							processedFile.LocalUrl = folderName + fileName;
						else
							processedFile.LocalUrl = folderName + processedFile.Width + PIC_PX + fileName;
					}
			}
		}


		/// <summary>
		/// Deletes all recs from "Recommendations" folder
		/// </summary>
		public static void EmptyRecommendations()
		{
			var dirs = Directory.EnumerateDirectories(Default.AppDataFolder + DIR_RECS);

			foreach (var dir in dirs)
			{
				Directory.Delete(dir);
			}
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
				foreach (var photo in photos)
				{
					Parallel.ForEach(photo.ProcessedFiles, processedPhoto =>
					{
						string downloadPath = picDir;
						// 620px photos go to the main pic folder, smaller pictures into folders
						downloadPath += processedPhoto.Height != 640 ? processedPhoto.Height + PIC_PX : "";
						downloadPath += photo.FileName ?? photo.Id + ".jpg";

						processedPhoto.LocalUrl = downloadPath;

						// Downloads only if the file does not yet exist
						if (!File.Exists(downloadPath))
							try
							{
								new WebClient().DownloadFile(new Uri(processedPhoto.Url), downloadPath);
								processedPhoto.LocalUrl = downloadPath;
							}
							catch
							{
								// Some pictures throw 403 forbidden, hell knows why
							}
					});
				}
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
				Directory.CreateDirectory(dir + IG_PICS);
				Directory.CreateDirectory(dir + IG_PICS + IG_THUMBS);

				string instaDir = dir + IG_PICS;
				Parallel.ForEach(instagram.InstagramPhotos, photo =>
				{
					string fileName = photo.Link.Substring(28, photo.Link.Length - 28 - 2) + ".jpg";

					// Downloads only if the file does not yet exist
					if (!File.Exists(instaDir + fileName))
					{
						try
						{
							new WebClient().DownloadFile(new Uri(photo.Image), instaDir + fileName);
							new WebClient().DownloadFile(new Uri(photo.Thumbnail), instaDir + IG_THUMBS + fileName);

							photo.LocalImage = instaDir + fileName;
							photo.LocalThumbnail = instaDir + IG_THUMBS + fileName;
						}
						catch
						{
							// In case we also get 403 here, we just skip those pics who cares
						}
					}
				});
				//foreach (var pic in instagram.InstagramPhotos)
				//{
				//	string fileName = pic.Link.Substring(28, pic.Link.Length - 28 - 2) + ".jpg";

				//	// Downloads only if the file does not yet exist
				//	if (!File.Exists(instaDir + fileName))
				//	{
				//		try
				//		{
				//			_webClient.DownloadFile(new Uri(pic.Image), instaDir + fileName);
				//			_webClient.DownloadFile(new Uri(pic.Thumbnail), instaDir + IG_THUMBS + fileName);
				//		}
				//		catch
				//		{
				//			// In case we also get 403 here, we just skip those pics who cares
				//		}
				//	}

				//}
			}
		}

		/// <summary>
		/// Creates folders for data saving
		/// </summary>
		/// <param name="root"></param>
		public static void CreateFolderStructure(string root)
		{
			Directory.CreateDirectory(Default.AppDataFolder);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_MATCHES);

			Directory.CreateDirectory(Default.AppDataFolder + DIR_UNMATCHED);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_UNMATCHED_BY_ME);

			Directory.CreateDirectory(Default.AppDataFolder + DIR_RECS);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_RECS_PENDING);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_RECS_PASSED);

			Directory.CreateDirectory(Default.AppDataFolder + DIR_USER);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_USER + PHOTOS);
		}

		/// <summary>
		/// Creates folders for match data
		/// </summary>
		/// <param name="root"></param>
		private static void CreateMatchFolder(string root)
		{
			Directory.CreateDirectory(root);
			Directory.CreateDirectory(root + PHOTOS);
			Directory.CreateDirectory(root + PHOTOS + PIC_84PX);
			Directory.CreateDirectory(root + PHOTOS + PIC_172PX);
			Directory.CreateDirectory(root + PHOTOS + PIC_320PX);
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
}
