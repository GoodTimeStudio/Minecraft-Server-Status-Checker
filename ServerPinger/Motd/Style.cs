using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodTimeStudio.ServerPing.Motd
{
    class Style
    {
        /// <summary>
        /// The parent of this ChatStyle.  Used for looking up values that this instance does not override.
        /// </summary>
        private Style parentStyle;
        private TextFormatting color;
        private Boolean bold;
        private Boolean italic;
        private Boolean underlined;
        private Boolean strikethrough;
        private Boolean obfuscated;
        private String insertion;
    }
}
