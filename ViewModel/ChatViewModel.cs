using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;
using Twinder.Helpers;
using Twinder.Model;
using Twinder.View;

namespace Twinder.ViewModel
{
	// TODO this all needs much better way of implementation
	public class ChatViewModel : ViewModelBase
	{
		private DateTime _lastActivity;
		private DispatcherTimer _timer; 
		private int _lastMessageIndex;

		public event NewChatMessageHandler NewChatMessageReceived;
		public delegate void NewChatMessageHandler(object sender, EventArgs e);

		public ChatView MyChatView { get; set; }

		private MatchModel _match;
		public MatchModel Match
		{
			get { return _match; }
			set
			{
				Set(ref _match, value);
				if (value != null)
				{
					MatchedAgo = value.CreatedDate.ToLocalTime().TimeAgo();

					_lastMessageIndex = value.Messages.Count - 1;
					if (_lastMessageIndex < 0)
						MessageAgo = "never";
					else
						timer_Tick(this, null);

					if (MessageAgo == MatchedAgo)
						AreMatchedAndMessagedEqual = true;

					StartDispatcherTimer();
					Match.Messages.CollectionChanged += Messages_CollectionChanged;
				}
			}
		}

		private string _matchedAgo;
		public string MatchedAgo
		{
			get { return _matchedAgo; }
			set { Set(ref _matchedAgo, value); }
		}

		private string _messageAgo;
		public string MessageAgo
		{
			get { return _messageAgo; }
			set { Set(ref _messageAgo, value); }
		}

		private bool _matchedAndMessagedEqual;
		public bool AreMatchedAndMessagedEqual
		{
			get { return _matchedAndMessagedEqual; }
			set { Set(ref _matchedAndMessagedEqual, value); }
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
			
			_timer = new DispatcherTimer();
			_lastActivity = DateTime.Now;
		}
		

		#region Timer
		private void StartDispatcherTimer()
		{
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += timer_Tick;
			timer.Start();
		}

		/// <summary>
		/// Updates MessageAgo with current time
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_Tick(object sender, EventArgs e)
		{
			if (_lastMessageIndex >= 0)
			{
				MessageAgo = Match.Messages[_lastMessageIndex].SentDate.ToLocalTime().TimeAgo();

				// Sets new update time
				var timeSpan = DateTime.Now.Subtract(Match.Messages[_lastMessageIndex].SentDate.ToLocalTime());

				if (timeSpan <= TimeSpan.FromSeconds(60))
					_timer.Interval = TimeSpan.FromSeconds(10);

				else if (timeSpan <= TimeSpan.FromMinutes(60))
					_timer.Interval = TimeSpan.FromMinutes(1);

				else if (timeSpan <= TimeSpan.FromHours(24))
					_timer.Interval = TimeSpan.FromHours(1);
			}
		}
		#endregion

		#region Send message command
		private async void SendMessage()
		{
			string msg = MessageToSend;
			try
			{
				MessageToSend = string.Empty;
				MessageModel sentMessage = await TinderHelper.SendMessage(Match.Id, msg);
				Match.Messages.Add(sentMessage);
				_lastActivity = sentMessage.SentDate;
			}
			catch (TinderRequestException e)
			{
				MessageBox.Show(e.Message);
				MessageToSend = msg;
			}
		}

		private bool CanSendMessage()
		{
			return !string.IsNullOrWhiteSpace(MessageToSend);
		}
		#endregion

		
		private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (MessageModel item in e.NewItems)
				{
					_lastMessageIndex = Match.Messages.Count - 1;

					// Invokes event if the new message is not from me
					if (!IsMessageFromMe(item))
						NewChatMessageReceived.Invoke(this, null);

					AreMatchedAndMessagedEqual = false;
					MyChatView.Update();

					Match.LastActivityDate = item.SentDate;
					timer_Tick(this, null);

					Messenger.Default.Send("", MessengerToken.SortMatchList);
				}
		}

		private bool IsMessageFromMe(MessageModel msg)
		{
			return Match.Person.Id != msg.From;
		}
		// Copying several messages might end up in this format

		//private string _chat;
		//public string Chat
		//{
		//	get { return _chat; }
		//	set { Set(ref _chat, value); }
		//}
		///// <summary>
		///// Creates chat in format:<para/> 
		///// [yyyy-MM-dd]<para/>
		///// [HH:mm:ss] &lt;Sender&gt; message
		///// </summary>
		//private void SetChat()
		//{
		//	Chat = string.Empty;
		//	for (int i = 0; Match != null &&	 i < Match.Messages.Count; i++)
		//	{
		//		var msg = Match.Messages[i];
		//		var sentDate = msg.SentDate.ToLocalTime().ToLongDateString();

		//		// Prints new date in new line if it's the first message or the date has changed
		//		if (i == 0 || (i > 0 && Match.Messages[i - 1].SentDate.ToLocalTime().Date != msg.SentDate.ToLocalTime().Date))
		//			Chat += string.Format($"[{sentDate}]\n");

		//		AddMessageToChat(msg);
		//	}
		//}

		///// <summary>
		///// Adds message to chat in format
		///// [HH:mm:ss] &lt;Message&gt; Message
		///// </summary>
		///// <param name="msg"></param>
		//private void AddMessageToChat(MessageModel msg)
		//{
		//	var sentTime = msg.SentDate.ToLocalTime().ToLongTimeString();

		//	var from = Match.Person.Name;
		//	// If message is from myself, change the sender to "Me"
		//	if (IsMessageFromMe(msg))
		//		from = "Me";

		//	var message = msg.Message;

		//	Chat += string.Format($"[{sentTime}] <{from}> {message}\n");
		//}
	}
}
