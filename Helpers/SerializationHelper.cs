using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Twinder.Model;
using Twinder.Model.Photos;
using static Twinder.Properties.Settings;
using System.ComponentModel;
using System.Security.AccessControl;

namespace Twinder.Helpers
{
	public static class SerializationHelper
	{
		private const string EXT = ".json";
		private const string MESSAGES = ".msgs";

		private const string DIR_MATCHES = "Matches\\";
		private const string DIR_MATCHES_DEL = "Matches-Unmatched\\";

		private const string DIR_RECS = "Recommendations\\";
		private const string DIR_RECS_PENDING = "Recommendations-Pending\\";
		private const string DIR_RECS_PASSED = "Recommendations-Passed\\";

		private const string DIR_USER = "User\\";

		private const string MATCH_PHOTOS = "Photos\\";
		private const string MATCH_IG_PICS = "Instagram\\";
		private const string MATCH_IG_THUMBS = "Thumbnails\\";

		private const string PIC_PX = "px\\";
		private const string PIC_84PX = "84px\\";
		private const string PIC_172PX = "172px\\";
		private const string PIC_320PX = "320px\\";

		private static WebClient _webClient;


		private static void SetWebClient()
		{
			_webClient = new WebClient();
			_webClient.Proxy = new WebProxy();
		}

		public static void SerializeUser(UserModel user, BackgroundWorker worker)
		{
			if (_webClient == null)
				SetWebClient();

			if (worker != null)
				worker.ReportProgress(1, 1);

			string workingDir = Default.AppDataFolder + DIR_USER;

			Directory.CreateDirectory(workingDir + MATCH_PHOTOS);
			Directory.CreateDirectory(workingDir + MATCH_PHOTOS + PIC_84PX);
			Directory.CreateDirectory(workingDir + MATCH_PHOTOS + PIC_172PX);
			Directory.CreateDirectory(workingDir + MATCH_PHOTOS + PIC_320PX);

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
			if (_webClient == null)
				SetWebClient();

			string workingDir = Default.AppDataFolder + DIR_MATCHES + match.ToString() + "\\";

			// Match doesn't have his own data folder
			if (!Directory.Exists(workingDir))
				CreateMatchFolder(workingDir);


			DownloadPhotos(match.Person.Photos, workingDir);
			DownloadInstagramPhotos(match.Instagram, workingDir);

			// Writes messages
			File.WriteAllText(workingDir + match.Person.Name + MESSAGES + EXT,
				JsonConvert.SerializeObject(match.Messages, Formatting.Indented));

			// Writes match data
			File.WriteAllText(workingDir + match.Person.Name + EXT,
				JsonConvert.SerializeObject(match, Formatting.Indented));
		}

		/// <summary>
		/// Serializes a given match in it's own folder
		/// </summary>
		/// <param name="rec"></param>
		public static void SerializeRecommendation(RecModel rec)
		{
			if (_webClient == null)
				SetWebClient();

			string workingDir = Default.AppDataFolder + DIR_RECS + rec.ToString() + "\\";

			// Rec doesn't have his own data folder
			if (!Directory.Exists(workingDir))
				CreateMatchFolder(workingDir);

			DownloadPhotos(rec.Photos, workingDir);
			DownloadInstagramPhotos(rec.Instagram, workingDir);

			File.WriteAllText(workingDir + rec.Name + EXT,
				JsonConvert.SerializeObject(rec, Formatting.Indented));
		}

		/// <summary>
		/// Serializes all matches
		/// </summary>
		/// <param name="matchList"></param>
		public static void SerializeMatchList(ObservableCollection<MatchModel> matchList, BackgroundWorker worker)
		{
			for (int i = 0; i < matchList.Count; i++)
			{
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
		public static void MoveRecToPending(RecModel rec)
		{
			var fromDir = Default.AppDataFolder + DIR_RECS + rec;
			var toDir = Default.AppDataFolder + DIR_RECS_PENDING + rec;

			Directory.Move(fromDir, toDir);
		}

		/// <summary>
		/// Moves recommendation to "Passed recommendations" directory
		/// </summary>
		/// <param name="rec"></param>
		public static void MoveRecToPassed(RecModel rec)
		{
			var fromDir = Default.AppDataFolder + DIR_RECS + rec;
			var toDir = Default.AppDataFolder + DIR_RECS_PASSED + rec;

			try
			{
				Directory.Move(fromDir, toDir);
			}
			catch (UnauthorizedAccessException e)
			{

			}
		}
		
		
		public static void MoveRecToMatches(RecModel rec)
		{
			MoveRecPhotosToMatches(rec.ToString(), DIR_RECS);
		}

		/// <summary>
		/// Moves photos of recommendation from "Pending recommendations" to "Matches" directory
		/// </summary>
		/// <param name="rec"></param>
		public static void MoveRecFromPendingToMatches(RecModel rec)
		{
			MoveRecPhotosToMatches(rec.ToString(), DIR_RECS_PENDING);
		}

		private static void MoveRecPhotosToMatches(string recName, string sourceDir)
		{
			var fromRecDir = Default.AppDataFolder + sourceDir + recName + "\\";
			var fromDirPhotos = fromRecDir + MATCH_PHOTOS;
			var fromDirIG = fromRecDir + MATCH_IG_PICS;

			var toRecDir = Default.AppDataFolder + DIR_MATCHES + recName + "\\";
			var toDirPhotos = toRecDir + MATCH_PHOTOS;
			var toDirIG = toRecDir + MATCH_IG_PICS;
			Directory.CreateDirectory(toRecDir);

			if (Directory.Exists(fromDirPhotos))
				Directory.Move(fromDirPhotos, toDirPhotos);

			if (Directory.Exists(fromDirIG))
				Directory.Move(fromDirIG, toDirIG);

			Directory.Delete(fromRecDir, true);
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
			var match = JsonConvert.DeserializeObject<MatchModel>(File.ReadAllText(folderName + "\\" + matchName + EXT));
			match.Messages = JsonConvert.DeserializeObject<ObservableCollection<MessageModel>>(
				File.ReadAllText(folderName + "\\" + matchName + MESSAGES + EXT)) ?? new ObservableCollection<MessageModel>();
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
			//var match = JsonConvert.DeserializeObject<MatchModel>(File.ReadAllText(folderName + "\\" + recName + EXT));
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
				string picDir = dir + MATCH_PHOTOS;
				foreach (var photo in photos)
				{
					Parallel.ForEach(photo.ProcessedFiles, processedPhoto =>
					{
						string downloadPath = picDir;
						// 620px photos go to the main pic folder, smaller pictures into folders
						downloadPath += processedPhoto.Height != 640 ? processedPhoto.Height + PIC_PX : "";
						downloadPath += photo.FileName ?? photo.Id + ".jpg";

						// Downloads only if the file does not yet exist
						if (!File.Exists(downloadPath))
							try
							{
								new WebClient().DownloadFile(new Uri(processedPhoto.Url), downloadPath);
							}
							catch
							{
								// Some pictures throw 403 forbidden, hell knows why
							}
						// If it exists, jumps to next photo, skips other processed files
						//else
						//	break;
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
				Directory.CreateDirectory(dir + MATCH_IG_PICS);
				Directory.CreateDirectory(dir + MATCH_IG_PICS + MATCH_IG_THUMBS);

				string instaDir = dir + MATCH_IG_PICS;
				Parallel.ForEach(instagram.InstagramPhotos, photo =>
				{
					string fileName = photo.Link.Substring(28, photo.Link.Length - 28 - 2) + ".jpg";

					// Downloads only if the file does not yet exist
					if (!File.Exists(instaDir + fileName))
					{
						try
						{
							new WebClient().DownloadFile(new Uri(photo.Image), instaDir + fileName);
							new WebClient().DownloadFile(new Uri(photo.Thumbnail), instaDir + MATCH_IG_THUMBS + fileName);
						}
						catch
						{
							// In case we also get 403 here, we just skip those pics who cares
						}
					}
				});
				foreach (var pic in instagram.InstagramPhotos)
				{
					string fileName = pic.Link.Substring(28, pic.Link.Length - 28 - 2) + ".jpg";

					// Downloads only if the file does not yet exist
					if (!File.Exists(instaDir + fileName))
					{
						try
						{
							_webClient.DownloadFile(new Uri(pic.Image), instaDir + fileName);
							_webClient.DownloadFile(new Uri(pic.Thumbnail), instaDir + MATCH_IG_THUMBS + fileName);
						}
						catch
						{
							// In case we also get 403 here, we just skip those pics who cares
						}
					}

				}
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
			Directory.CreateDirectory(Default.AppDataFolder + DIR_MATCHES_DEL);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_RECS);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_RECS_PENDING);
			Directory.CreateDirectory(Default.AppDataFolder + DIR_USER);
		}

		/// <summary>
		/// Creates folders for match data
		/// </summary>
		/// <param name="root"></param>
		private static void CreateMatchFolder(string root)
		{
			Directory.CreateDirectory(root);
			Directory.CreateDirectory(root + MATCH_PHOTOS);
			Directory.CreateDirectory(root + MATCH_PHOTOS + PIC_84PX);
			Directory.CreateDirectory(root + MATCH_PHOTOS + PIC_172PX);
			Directory.CreateDirectory(root + MATCH_PHOTOS + PIC_320PX);
		}
	}

	/// <summary>
	/// For use when sending from ViewModel multiple values to serialize
	/// </summary>
	public sealed class SerializationPacket
	{
		public ObservableCollection<MatchModel> MatchList { get; set; }
		public ObservableCollection<RecModel> RecList { get; set; }
		public UserModel User { get; set; }

		public SerializationPacket(ObservableCollection<MatchModel> matchList)
		{
			MatchList = matchList;
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
