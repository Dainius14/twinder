using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twinder.Helpers;
using Twinder.Model.Updates.Match;
using Twinder.Models.Updates;

namespace Twinder.ViewModel
{
	public class ChatViewModel : ViewModelBase
	{
		private DateTime _lastActivity;

		private MatchModel _match;
		public MatchModel Match
		{
			get { return _match; }
			set
			{
				Set(ref _match, value);
				SimplifyMessages();
				SetChat();
			}
		}

		private string _chat;
		public string Chat
		{
			get { return _chat; }
			set { Set(ref _chat, value); }
		}

		private string _messageToSend;
		public string MessageToSend
		{
			get { return _messageToSend; }
			set { Set(ref _messageToSend, value); }
		}

		public RelayCommand SendMessageCommand { get; private set; }
		public RelayCommand UpdateCommand { get; private set; }

		public ChatViewModel()
		{
			SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
			UpdateCommand = new RelayCommand(Update);


		}

		// Sends a request to server for updates on chat model
		private void Update()
		{
			if (_lastActivity == default(DateTime))
				_lastActivity = Match.LastActivityDate;

			var newUpdates = TinderHelper.GetUpdates(_lastActivity);
			if (newUpdates.Matches.Count > 0)
			{
				var matchUpdates = newUpdates.Matches.Where(item => item.Id == Match.Id).FirstOrDefault();

				if (matchUpdates != null)
					foreach (var msg in matchUpdates.Messages)
						if (!Match.Messages.Contains(msg))
						{
							Match.Messages.Add(msg);
							AddMessageToChat(msg);
						}
			}

		}

		#region Send message command
		private bool CanSendMessage()
		{
			return !string.IsNullOrWhiteSpace(MessageToSend);
		}

		private void SendMessage()
		{
			MessageModel sentMessage = TinderHelper.SendMessage(Match.Id, MessageToSend);
			if (sentMessage != null)
			{
				MessageToSend = string.Empty;
				Match.Messages.Add(sentMessage);
				AddMessageToChat(sentMessage);
				_lastActivity = sentMessage.SentDate;
			}
		}
		#endregion

		/// <summary>
		/// Generated a list of messages with only three neccessary properties
		/// </summary>
		private void SimplifyMessages()
		{
			var messages = Match.Messages.Select(item => new SimpleMessageModel()
			{
				Message = item.Message,
				SentDate = item.SentDateLocal,
				// Sets name to either the name of the match or me
				From = Match.Person.Id == item.From ? Match.Person.Name : "Me"
			});
		}

		/// <summary>
		/// Creates chat in format 
		/// [yyyy-MM-dd]
		/// [HH:mm:ss] <Sender> message
		/// </summary>
		private void SetChat()
		{
			Chat = string.Empty;
			for (int i = 0; i < Match.Messages.Count; i++)
			{
				var msg = Match.Messages[i];
				var sentDate = msg.SentDateLocal.ToLongDateString();

				// Prints new date in new line if it's the first message or the date has changed
				if (i == 0 || (i > 0 && Match.Messages[i - 1].SentDateLocal.Date != msg.SentDateLocal.Date))
					Chat += string.Format($"[{sentDate}]\n");

				AddMessageToChat(msg);
			}
		}

		private void AddMessageToChat(MessageModel msg)
		{
			var sentTime = msg.SentDateLocal.ToLongTimeString();
			var from = Match.Person.Id == msg.From ? Match.Person.Name : "Me";
			var message = msg.Message;
			
			Chat += string.Format($"[{sentTime}] <{from}> {message}\n");
		}
	}
}
