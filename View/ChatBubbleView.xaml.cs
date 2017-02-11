using GalaSoft.MvvmLight;
using System;
using System.Linq;
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



		public bool IsCopyEnabled
		{
			get { return (bool) GetValue(IsCopyEnabledProperty); }
			set { SetValue(IsCopyEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsCopyEnabledProperty =
			DependencyProperty.Register("IsCopyEnabled", typeof(bool), typeof(ChatBubbleView));



		public ChatBubbleView()
		{
			InitializeComponent();
			Loaded += SetUpBubble;
		}

		public void SetUpBubble(object sender, EventArgs e)
		{
			if (Match != null && Message != null)
			{
				// Message from match
				if (Match.Person.Id == Message.From)
				{
					if (IsFromTheSameAsPrevious())
					{
						LowerTail.Visibility = Visibility.Collapsed;
						UpperTail.Visibility = Visibility.Collapsed;
					}
					else
					{
						LowerTail.Visibility = Visibility.Collapsed;
						UpperTail.Visibility = Visibility.Visible;
					}
					HorizontalAlignment = HorizontalAlignment.Left;
					BubbleBorder.Style = (Style) FindResource("MatchColor");

					Grid.SetColumn(BubbleBorder, 0);
					Grid.SetColumn(UpperTail, 0);
					Grid.SetColumn(Date_TextBlock, 1);
					Date_TextBlock.TextAlignment = TextAlignment.Left;
				}
				// Match from user
				else
				{
					if (IsFromTheSameAsNext())
					{
						LowerTail.Visibility = Visibility.Collapsed;
						UpperTail.Visibility = Visibility.Collapsed;
					}
					else
					{
						LowerTail.Visibility = Visibility.Visible;
						UpperTail.Visibility = Visibility.Collapsed;
					}
					HorizontalAlignment = HorizontalAlignment.Right;
					BubbleBorder.Style = (Style) FindResource("UserColor");

					Grid.SetColumn(BubbleBorder, 1);
					Grid.SetColumn(LowerTail, 1);
					Grid.SetColumn(Date_TextBlock, 0);
					Date_TextBlock.TextAlignment = TextAlignment.Right;
				}
			}
		}

		/// <summary>
		/// Checks if current and previous message are from the same person
		/// </summary>
		/// <returns></returns>
		private bool IsFromTheSameAsPrevious()
		{
			int messageIndex = Match.Messages.IndexOf(Message);
			if (messageIndex == 0)
				return false;
			return Match.Messages[messageIndex - 1].From == Message.From;
		}

		/// <summary>
		/// Checks if current and next message are from the same person
		/// </summary>
		/// <returns></returns>
		private bool IsFromTheSameAsNext()
		{
			int messageIndex = Match.Messages.IndexOf(Message);
			if (messageIndex == Match.Messages.Count - 1)
				return false;
			return Match.Messages[messageIndex + 1].From == Message.From;
		}

		private void Bubble_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Date_TextBlock.Visibility = Visibility.Visible;
		}

		private void Bubble_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Date_TextBlock.Visibility = Visibility.Collapsed;
		}

		//private void ContextMenu_Copy_Click(object sender, RoutedEventArgs e)
		//{
		//	Message_TextBox.Copy();
		//}

		//private void ContextMenu_CopyAll_Click(object sender, RoutedEventArgs e)
		//{
		//	Message_TextBox.SelectAll();
		//	Message_TextBox.Copy();
		//	Message_TextBox.Select(0, 0);
		//}
	}
}
