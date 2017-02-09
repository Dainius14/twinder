using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Twinder.Model;

namespace Twinder.View
{
	public partial class ChatBubbleView : UserControl
	{

		public MatchModel Match
		{
			get { return (MatchModel) GetValue(MatchProperty); }
			set { SetValue(MatchProperty, value); }
		}

		public static readonly DependencyProperty MatchProperty =
			DependencyProperty.Register("Match", typeof(MatchModel), typeof(ChatBubbleView));



		public MessageModel Message
		{
			get { return (MessageModel) GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}

		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(MessageModel), typeof(ChatBubbleView));

		public ChatBubbleView()
		{
			InitializeComponent();
			Loaded += SetTailVisibility;
		}

		private void SetTailVisibility(object sender, EventArgs e)
		{
			if (Match != null && Message != null)
			{
				// Message from match
				if (Match.Person.Id == Message.From)
				{
					LowerTail.Visibility = Visibility.Collapsed;
					UpperTail.Visibility = Visibility.Visible;
					HorizontalAlignment = HorizontalAlignment.Left;
					BubbleBorder.Style = (Style) FindResource("MatchColor");
				}
				// Match from user
				else
				{
					LowerTail.Visibility = Visibility.Visible;
					UpperTail.Visibility = Visibility.Collapsed;
					HorizontalAlignment = HorizontalAlignment.Right;
					BubbleBorder.Style = (Style) FindResource("UserColor");
				}
			}
		}
	}
}
