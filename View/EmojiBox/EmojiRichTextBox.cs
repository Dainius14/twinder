using GalaSoft.MvvmLight;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Twinder.Model;

namespace Twinder.Views.EmojiBox
{
	public class EmojiRichTextBox : RichTextBox
	{
		bool readingInput = false;
		string input = "";
		EmojiParser ep = new EmojiParser();
		TextPointer inputStart = null;

		private int _trueLength;
		private bool _emojisExist;
		private string _msg;
		private string _msgWithoutEmojies = string.Empty;

		public EmojiRichTextBox()
		{
			if (ViewModelBase.IsInDesignModeStatic)
				return;

			this.Loaded += EmojiRichTextBox_Loaded;
		}

		private void EmojiRichTextBox_Loaded(object sender, RoutedEventArgs e)
		{
			_msg = (DataContext as MessageModel).Message;

			// Sets width of control. This is an approximation
			FormattedText txt = new FormattedText(_msg,
				CultureInfo.CurrentCulture, Document.FlowDirection,
				new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
				FontSize + 2, Foreground);


			txt.MaxTextWidth = MaxWidth - 10;


			ParseText(_msg);


			Width = txt.Width;
			if (Width < MinWidth)
				Width = MinWidth + 2;
			if (Width + 2 > MaxWidth)
				Width = MaxWidth - 2;

			Document.PageWidth = Width;

			DataObject.AddCopyingHandler(this, OnCopy);
		}

		private void ParseText(string text)
		{
			var inlines = (Document.Blocks.FirstBlock as Paragraph).Inlines;

			string textPart = string.Empty;

			for (int i = 0; i < text.Length; i++)
			{
				_trueLength++;

				if (char.IsSurrogatePair(text, i) && i + 1 < text.Length)
				{
					int codepoint = char.ConvertToUtf32(text.Substring(i, 2), 0);
					try
					{
						BitmapImage emoji = ep.GetEmojiWithUnicodeDec(codepoint);

						_emojisExist = true;
						_msgWithoutEmojies += " ";

						// Adding preceeding text
						inlines.Add(new Run(textPart));
						textPart = string.Empty;

						// Add emoji
						inlines.Add(CreateEmoji(emoji, FontSize));

						i++;
					}
					catch
					{
						// Emoji not found
					}
				}
				else
				{
					textPart += text[i];
					_msgWithoutEmojies += text[i];
				}
			}

			if (textPart != string.Empty)
				inlines.Add(new Run(textPart));
		}

		private Image CreateEmoji(BitmapImage bitmap, double size)
		{
			Image img = new Image();
			img.Source = bitmap;
			img.UseLayoutRounding = true;
			RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality);
			img.Height = size + 1;
			img.Width = size + 1;
			img.VerticalAlignment = VerticalAlignment.Bottom;
			return img;
		}

		/// <summary>
		/// Handles Copy event, because text contains images and we need to substitute that
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCopy(object sender, DataObjectCopyingEventArgs e)
		{
			if (!_emojisExist)
				return;

			string copied = e.DataObject.GetData(DataFormats.UnicodeText) as string;

			// Full message
			if (copied.Length == _trueLength)
			{
				Clipboard.SetText(_msg, TextDataFormat.UnicodeText);
			}
			else
			{
				int start = GetSelectionStart();
				int length = GetSelectionLength();
				
				string toCopy = _msg.Substring(start, length);

				Clipboard.SetText(toCopy, TextDataFormat.UnicodeText);
			}

			e.Handled = true;
			e.CancelCommand();
		}
		public int GetSelectionStart()
		{
			var navigator = Document.ContentStart;
			var navigatorEnd = Selection.Start;
			int cnt = 0;
			while (navigator.CompareTo(navigatorEnd) < 0)
			{
				switch (navigator.GetPointerContext(LogicalDirection.Forward))
				{
					case TextPointerContext.EmbeddedElement:
						cnt += 2;
						break;
					case TextPointerContext.Text:
						cnt++;
						break;
				}
				navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

			} // End while.

			return cnt;
		}

		public int GetSelectionLength()
		{
			var navigator = Selection.Start;
			var navigatorEnd = Selection.End;
			int cnt = 0;

			while (navigator.CompareTo(navigatorEnd) < 0)
			{
				switch (navigator.GetPointerContext(LogicalDirection.Forward))
				{
					case TextPointerContext.EmbeddedElement:
						cnt += 2;
						break;
					case TextPointerContext.Text:
						cnt++;
						break;
				}

				navigator = navigator.GetPositionAtOffset(1, LogicalDirection.Forward);

			} // End while.

			return cnt;
		}
	}
}
