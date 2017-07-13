using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Minecraft_Server_Status_Checker.Status
{

    //http://minecraft.gamepedia.com/Formatting_codes
    public class ColorCode
    {
        public static readonly ColorCode Black = new ColorCode("§0", Colors.Black);
        public static readonly ColorCode DarkBlue = new ColorCode("§1", Colors.DarkBlue);
        public static readonly ColorCode DarkGreen = new ColorCode("§2", Colors.DarkGreen);
        public static readonly ColorCode DarkAqua = new ColorCode("§3", Color.FromArgb(255, 0, 170, 170));
        public static readonly ColorCode DarkRed = new ColorCode("§4", Colors.DarkRed);
        public static readonly ColorCode DarkPurple = new ColorCode("§5", Color.FromArgb(255, 170, 0, 170));
        public static readonly ColorCode Gold = new ColorCode("§6", Colors.Gold);
        public static readonly ColorCode Gray = new ColorCode("§7", Colors.Gray);
        public static readonly ColorCode DarkGray = new ColorCode("§8", Colors.DarkGray);
        public static readonly ColorCode Blue = new ColorCode("§9", Colors.Blue);
        public static readonly ColorCode Green = new ColorCode("§a", Colors.Green);
        public static readonly ColorCode Aqua = new ColorCode("§b", Colors.Aqua);
        public static readonly ColorCode Red = new ColorCode("§c", Colors.Red);
        public static readonly ColorCode LightPurple = new ColorCode("§d", Color.FromArgb(255, 255, 85, 255));
        public static readonly ColorCode Yellow = new ColorCode("§e", Colors.Yellow);
        public static readonly ColorCode White = new ColorCode("§f", Colors.White);

        public static IEnumerable<ColorCode> Values
        {
            get
            {
                yield return Black;
                yield return DarkBlue;
                yield return DarkGreen;
                yield return DarkAqua;
                yield return DarkRed;
                yield return DarkPurple;
                yield return Gold;
                yield return Gray;
                yield return DarkGray;
                yield return Blue;
                yield return Green;
                yield return Aqua;
                yield return Red;
                yield return LightPurple;
                yield return Yellow;
                yield return White;
            }
        }

        public static ColorCode GetStyleColorFromCode(string colorcCode)
        {
            foreach (ColorCode style in Values)
            {
                if (style.colorCode.Equals(colorcCode))
                {
                    return style;
                }
            }
            return null;
        }

        public static bool IsColorCode(string code)
        {
            foreach (ColorCode style in Values)
            {
                if (style.colorCode.Equals(code))
                {
                    return true;
                }
            }
            return false;
        }

        public readonly string colorCode;
        public readonly Color color;

        private ColorCode(string colorCode, Color color)
        {
            this.colorCode = colorCode;
            this.color = color;
        }

    }

    public class FormattingCode
    {
        public static readonly FormattingCode Obfuscated = new FormattingCode("§k");
        public static readonly FormattingCode Bold = new FormattingCode("§l");
        public static readonly FormattingCode Strikethrough = new FormattingCode("§m");
        public static readonly FormattingCode Underline = new FormattingCode("§n");
        public static readonly FormattingCode Italic = new FormattingCode("§o");
        public static readonly FormattingCode Reset = new FormattingCode("§r");

        public static IEnumerable<FormattingCode> Values
        {
            get
            {
                yield return Obfuscated;
                yield return Bold;
                yield return Strikethrough;
                yield return Underline;
                yield return Italic;
                yield return Reset;
            }
        }

        public static FormattingCode GetTypeFromCode(string code)
        {
            foreach (FormattingCode formattingCode in Values)
            {
                if (formattingCode.code.Equals(code))
                {
                    return formattingCode;
                }
            }
            return null;
        }

        public static bool IsFormattingCode(string code)
        {
            foreach (FormattingCode formattingCode in Values)
            {
                if (formattingCode.code.Equals(code))
                {
                    return true;
                }
            }
            return false;
        }

        public readonly string code;

        private FormattingCode(string code)
        {
            this.code = code;
        }     
    }

}
