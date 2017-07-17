using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodTimeStudio.ServerPing.Motd
{
    class TextFormattingAttr : Attribute
    {
        /// <summary>
        /// The name of this color/formatting.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// The formatting code that produces this format.
        /// </summary>
        public readonly char formattingCode;

        public readonly bool fancyStyling;

        /// <summary>
        /// The control string (section sign + formatting code) that can be inserted into client-side text to display
        /// subsequent text in this format.
        /// </summary>
        public readonly string controlString;

        /// <summary>
        /// The numerical index that represents this color
        /// </summary>
        public readonly int colorIndex;

        public TextFormattingAttr(string FormattingName, char FormattingCodeIn, bool FancyStyleingIn, int ColorIndex)
        {
            this.name = FormattingName;
            this.formattingCode = FormattingCodeIn;
            this.fancyStyling = FancyStyleingIn;
        }

        public TextFormattingAttr(string FormattingName, char FormattingCodeIn, bool FancyStylingIn) : this(FormattingName, FormattingCodeIn, FancyStylingIn, -1)
        { }

        public TextFormattingAttr(string FormattingName, char FormattingCodeIn, int ColorIndex) : this(FormattingName, FormattingCodeIn, false, ColorIndex)
        { }

    }

    class TextFormattings
    {

    }

    enum TextFormatting
    {
    }
}
