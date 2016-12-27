using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Specialized;
using System.Windows;
using Twinder.Helpers;
using Twinder.Model;

namespace Twinder.ViewModel
{
	// TODO this all needs much better way of implementation
	public class ChatViewModel : ViewModelBase
	{
		private DateTime _lastActivity;

		public event NewChatMessageHandler NewChatMessageReceived;
		public delegate void NewChatMessageHandler(object sender, EventArgs e);

		private MatchModel _match;
		public MatchModel Match
		{
			get { return _match; }
			set
			{
				Set(ref _match, value);
				SetChat();
				Match.Messages.CollectionChanged += Messages_CollectionChanged;
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

		public ChatViewModel()
		{
			SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);

			_lastActivity = DateTime.Now;
			
		}
		
		#region Send message command
		private bool CanSendMessage()
		{
			return !string.IsNullOrWhiteSpace(MessageToSend);
		}

		private async void SendMessage()
		{
			try
			{
				MessageModel sentMessage = await TinderHelper.SendMessage(Match.Id, MessageToSend);
				MessageToSend = string.Empty;
				Match.Messages.Add(sentMessage);
				_lastActivity = sentMessage.SentDate;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
			}
		 }
		#endregion
		
		/// <summary>
		/// Creates chat in format:<para/> 
		/// [yyyy-MM-dd]<para/>
		/// [HH:mm:ss] &lt;Sender&gt; message
		/// </summary>
		private void SetChat()
		{
			Chat = string.Empty;
			for (int i = 0; i < Match.Messages.Count; i++)
			{
				var msg = Match.Messages[i];
				var sentDate = msg.SentDate.ToLocalTime().ToLongDateString();

				// Prints new date in new line if it's the first message or the date has changed
				if (i == 0 || (i > 0 && Match.Messages[i - 1].SentDate.ToLocalTime().Date != msg.SentDate.ToLocalTime().Date))
					Chat += string.Format($"[{sentDate}]\n");

				AddMessageToChat(msg);
			}
		}

		/// <summary>
		/// Adds message to chat in format
		/// [HH:mm:ss] &lt;Message&gt; Message
		/// </summary>
		/// <param name="msg"></param>
		private void AddMessageToChat(MessageModel msg)
		{
			var sentTime = msg.SentDate.ToLocalTime().ToLongTimeString();

			var from = Match.Person.Name;
			// If message is from myself, change the sender to "Me"
			if (IsMessageFromMe(msg))
				from = "Me";

			var message = msg.Message;
			
			Chat += string.Format($"[{sentTime}] <{from}> {message}\n");
		}

		private bool IsMessageFromMe(MessageModel msg)
		{
			return Match.Person.Id != msg.From;
		}

		private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (MessageModel item in e.NewItems)
				{
					AddMessageToChat(item);

					// Invokes event if the new message is not from me
					if (!IsMessageFromMe(item))
						NewChatMessageReceived.Invoke(this, null);


					Match.LastActivityDate = item.SentDate;

					Messenger.Default.Send("", MessengerToken.SortMatchList);


				}
		}

	}
}
