using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Twinder.Helpers;
using Twinder.Models;
using Twinder.Models.Updates;

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

			if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				//DispatcherTimer timer = new DispatcherTimer();
				//timer.Tick += new EventHandler(UpdateTick);
				//timer.Interval = new TimeSpan(0, 0, 5);
				//timer.Start();
			}
		}

	

		/// <summary>
		/// Sends a request to server for updates on chat
		/// </summary>
		private async void Update()
		{
			// fuck you designer mode
			if (!IsInDesignMode)
			{
				var newUpdates = await TinderHelper.GetUpdates(_lastActivity);

				// TODO better error handling
				if (newUpdates == null)
					MessageBox.Show("Error getting updates");

				if (newUpdates.Matches.Count > 0)
				{
					_lastActivity = newUpdates.LastActivityDateLocal;
					NewChatMessageReceived.Invoke(this, null);
					var matchUpdates = newUpdates.Matches.Where(item => item.Id == Match.Id).FirstOrDefault();

					if (matchUpdates != null)
						foreach (var msg in matchUpdates.Messages)
							// Checks if the new message doesn't already exist
							if (!Match.Messages.Contains(msg))
							{
								Match.Messages.Add(msg);
								AddMessageToChat(msg);
							}
					}
			}

		}


		private void UpdateTick(object sender, EventArgs e)
		{
			if (!IsInDesignMode)
				Update();
		}
		
		#region Send message command
		private bool CanSendMessage()
		{
			return !string.IsNullOrWhiteSpace(MessageToSend);
		}

		private async void SendMessage()
		{
			MessageModel sentMessage = await TinderHelper.SendMessage(Match.Id, MessageToSend);
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
				var sentDate = msg.SentDateLocal.ToLongDateString();

				// Prints new date in new line if it's the first message or the date has changed
				if (i == 0 || (i > 0 && Match.Messages[i - 1].SentDateLocal.Date != msg.SentDateLocal.Date))
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
			var sentTime = msg.SentDateLocal.ToLongTimeString();
			var from = Match.Person.Id == msg.From ? Match.Person.Name : "Me";
			var message = msg.Message;
			
			Chat += string.Format($"[{sentTime}] <{from}> {message}\n");
		}
	}
}
