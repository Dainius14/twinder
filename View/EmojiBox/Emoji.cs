using System.Collections.Generic;

namespace Twinder.Views.EmojiBox
{
	internal class Emoji
    {
        /// <summary>
        /// Gets or sets the Unicode code for this emoji.
        /// </summary>
        public string Unicode { get; set; }

        /// <summary>
        /// Gets or sets alternative Unicode codes for this emoji.
        /// </summary>
        public string UnicodeAlternatives { get; set; }

        /// <summary>
        /// Gets or sets the official name of this emoji.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a generally-agreed-upon short name for this emoji. This name will be surrounded by colons (i.e. ":smiley:").
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the category this emoji belongs in. Examples include "people", "travel", or "symbols".
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the numerical location of this emoji in the library.
        /// </summary>
        public int EmojiOrder { get; set; }

        /// <summary>
        /// Gets or sets a list of alias strings that correlate to this specific emoji. Each unique string could correlate to only one emoji.
        /// </summary>
        public List<string> Aliases { get; set; }

        /// <summary>
        /// Gets or sets a list of alias strings, with ASCII text, that correlate to this specific emoji.
        /// </summary>
        public List<string> AliasesAscii { get; set; }

        /// <summary>
        /// Gets or sets a list of keywords, to use for searching for and identifying this emoji.
        /// </summary>
        public List<string> Keywords { get; set; }

    }
}
