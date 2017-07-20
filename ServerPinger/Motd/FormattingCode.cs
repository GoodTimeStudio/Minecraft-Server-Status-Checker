using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Minecraft_Server_Status_Checker.Status.Motd
{

    //http://minecraft.gamepedia.com/Formatting_codes
    public class ColorCode
    {
        public static readonly ColorCode Black = new ColorCode("§0", Colors.Black, "black");
        public static readonly ColorCode DarkBlue = new ColorCode("§1", Colors.DarkBlue, "dark_blue");
        public static readonly ColorCode DarkGreen = new ColorCode("§2", Colors.DarkGreen, "dark_green");
        public static readonly ColorCode DarkAqua = new ColorCode("§3", Color.FromArgb(255, 0, 170, 170), "dark_aqua");
        public static readonly ColorCode DarkRed = new ColorCode("§4", Colors.DarkRed, "dark_red");
        public static readonly ColorCode DarkPurple = new ColorCode("§5", Color.FromArgb(255, 170, 0, 170), "dark_purple");
        public static readonly ColorCode Gold = new ColorCode("§6", Colors.Gold, "gold");
        public static readonly ColorCode Gray = new ColorCode("§7", Colors.Gray, "gray");
        public static readonly ColorCode DarkGray = new ColorCode("§8", Colors.DarkGray, "dark_gray");
        public static readonly ColorCode Blue = new ColorCode("§9", Colors.Blue, "blue");
        public static readonly ColorCode Green = new ColorCode("§a", Colors.Green, "green");
        public static readonly ColorCode Aqua = new ColorCode("§b", Colors.Aqua, "aqua");
        public static readonly ColorCode Red = new ColorCode("§c", Colors.Red, "red");
        public static readonly ColorCode LightPurple = new ColorCode("§d", Color.FromArgb(255, 255, 85, 255), "light_purple");
        public static readonly ColorCode Yellow = new ColorCode("§e", Colors.Yellow, "yellow");
        public static readonly ColorCode White = new ColorCode("§f", Colors.White, "white");

        public static readonly ColorCode DefaultColor = DarkGray;

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

        public static ColorCode GetColorCodeFromCode(string colorcCode)
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

        public static ColorCode GetColorCodeFromColorName(string name)
        {
            foreach (ColorCode code in Values)
            {
                if (code.name.Equals(name))
                {
                    return code;
                }
            }
            return DefaultColor; // TO-DO:   NO TEST, Default or White ?
        }

        public readonly string colorCode;
        public readonly Color color;
        public readonly string name;

        private ColorCode(string colorCode, Color color, string name)
        {
            this.colorCode = colorCode;
            this.color = color;
            this.name = name;
        }

    }

    public class StyleCode
    {
        public static readonly StyleCode Obfuscated = new StyleCode("§k");
        public static readonly StyleCode Bold = new StyleCode("§l");
        public static readonly StyleCode Strikethrough = new StyleCode("§m");
        public static readonly StyleCode Underline = new StyleCode("§n");
        public static readonly StyleCode Italic = new StyleCode("§o");
        public static readonly StyleCode Reset = new StyleCode("§r");

        public static IEnumerable<StyleCode> Values
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

        public static StyleCode GetTypeFromCode(string code)
        {
            foreach (StyleCode formattingCode in Values)
            {
                if (formattingCode.code.Equals(code))
                {
                    return formattingCode;
                }
            }
            return null;
        }

        public static bool IsStyleCode(string code)
        {
            foreach (StyleCode formattingCode in Values)
            {
                if (formattingCode.code.Equals(code))
                {
                    return true;
                }
            }
            return false;
        }

        public readonly string code;

        private StyleCode(string code)
        {
            this.code = code;
        }     
    }

}
