using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Minecraft_Server_Status_Checker.Status.Motd
{
    public sealed partial class TextComponentBlock : UserControl
    {
        public TextComponent _Content;
        public new TextComponent Content
        {
            set
            {
                _Content = value;
                ParseAndShowTextComponent();
            }
            get
            {
                return _Content;
            }
        }

        public TextComponentBlock()
        {
            this.InitializeComponent();
        }

        private void ParseAndShowTextComponent()
        {
            TextComponentBase BaseStyle = _Content as TextComponentBase;
            Paragraph paragraph = new Paragraph();
            
            foreach (TextComponent sub in _Content.extra)
            {
                Run run = new Run();
                run.Text = sub.text;

                if (sub.color != null)
                {
                    run.Foreground = new SolidColorBrush(sub.color.color);
                }
                else
                {
                    if (BaseStyle.color != null)
                    {
                        run.Foreground = new SolidColorBrush(BaseStyle.color.color);
                    }
                    else
                    {
                        run.Foreground = new SolidColorBrush(ColorCode.DefaultColor.color);
                    }
                }

                if (CheckStyle(sub.bold, BaseStyle.bold))
                    run.FontWeight = FontWeights.Bold;
                if (CheckStyle(sub.italic, BaseStyle.italic))
                    run.FontStyle = FontStyle.Italic;
                if (CheckStyle(sub.underlined, BaseStyle.underlined))
                    run.TextDecorations = TextDecorations.Underline;
                if (CheckStyle(sub.strikethrough, BaseStyle.strikethrough))
                    run.TextDecorations = TextDecorations.Strikethrough;
                if (CheckStyle(sub.obfuscated, BaseStyle.obfuscated))
                {
                    //TO-DO: UI Element
                }

                paragraph.Inlines.Add(run);
            }

            TextBlock.Blocks.Add(paragraph);
        }

        private bool CheckStyle(bool? style, bool? BaseStyle)
        {
            if (style != null)
            {
                if (style == true)
                    return true;
            }
            else
            {
                if (BaseStyle != null)
                {
                    if (BaseStyle == true)
                        return true;
                }
            }

            return false;
        }
    }
}
