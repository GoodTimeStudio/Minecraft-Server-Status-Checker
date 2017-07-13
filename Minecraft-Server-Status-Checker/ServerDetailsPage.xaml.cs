using Minecraft_Server_Status_Checker.Status;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Minecraft_Server_Status_Checker
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ServerDetailsPage : Page
    {
        private Server server;
        private ObservableCollection<Player> sample;
        private String RemainPlayersMsg;
        private Visibility RemainPlayersMsgVisibility = Visibility.Collapsed;

        public ServerDetailsPage()
        {
            this.InitializeComponent();
        }

        /// <param name="motd">no null</param>
        private void ParseAndShowMotd(string motd)
        {
            Regex reg = new Regex("§");
            //换行后格式样式将会重置，替换后方便下一步处理
            List<string> strs = SplitMotdToList(motd.Replace("\n", "\n§r"));

            foreach (string str in strs)
            {
                if (str.Equals("§e§lhome of\n"))
                    Debug.WriteLine("break");

                //+1 (last §)
                //+1 (last codeBody)
                string codes = str.Substring(0, str.LastIndexOf("§") + 2);
                string[] codeArray = codes.Split('§');
                string text = str.Substring(str.LastIndexOf("§") + 2);

                ColorCode color = null;
                List<FormattingCode> farmattings = new List<FormattingCode>();
                foreach (string codeBody in codeArray)
                {
                    if (FormattingCode.IsFormattingCode("§" + codeBody))
                    {
                        farmattings.Add(FormattingCode.GetTypeFromCode("§" + codeBody));
                    }
                    else
                    {
                        color = ColorCode.GetStyleColorFromCode("§" + codeBody);
                    }
                }

                Run run = new Run();
                Underline under = null;

                foreach (FormattingCode format in farmattings)
                {
                    if (format == FormattingCode.Obfuscated)
                    {

                    }
                    else if (format == FormattingCode.Bold)
                    {
                        run.FontWeight = FontWeights.Bold;
                    }
                    else if (format == FormattingCode.Strikethrough)
                    {

                    }
                    else if (format == FormattingCode.Underline)
                    {
                        under = new Underline();
                    }
                    else if (format == FormattingCode.Italic)
                    {
                        run.FontStyle = FontStyle.Italic;
                    }
                }

                //color
                if (color != null)
                {
                    SolidColorBrush brush = new SolidColorBrush(color.color);
                    run.Foreground = brush;
                }

                if (under != null)
                {

                }
                else
                {
                    MotdTextBlock.Inlines.Add(run);
                    run.Text = text;
                }
            }
        }

        /// <summary>
        /// 将Motd拆分成小段，方便后期处理
        /// </summary>
        private static List<string> SplitMotdToList(string motd)
        {
            List<string> ret = new List<string>();
            Regex reg = new Regex("§");

            //获取motd中样式代码的个数
            int codeCount = reg.Matches(motd).Count;

            //current
            string subStr;
            string codeStr;
            string text;

            int lastIndex = -1;
            string lastSingleColorCodeStr = "";
            List<string> formattingCodesStr = new List<string>();
            for (int i = 0; i < codeCount; i++)
            {
                int index = motd.IndexOf("§", lastIndex + 1);

                if (lastIndex >= 0 && index > 0)
                {

                    int count = index - lastIndex;
                    if (count >= 3)
                    {
                        subStr = motd.Substring(lastIndex, count);
                        codeStr = subStr.Substring(0, 2);
                        text = subStr.Substring(2);

                        //判断codeStr所属代码类型
                        FormattingCode format = FormattingCode.GetTypeFromCode(codeStr);
                        string formats = "";
                        if (format != null)
                        {
                            if (format == FormattingCode.Reset)
                            {
                                formattingCodesStr.Clear();
                                lastSingleColorCodeStr = "";
                            }
                            else
                            {
                                formattingCodesStr.Add(codeStr);
                                foreach (string str in formattingCodesStr)
                                {
                                    formats += str;
                                }
                            }
                        }
                        //format为空，则该代码不是格式代码
                        else
                        {
                            lastSingleColorCodeStr = codeStr;
                        }

                        ret.Add(lastSingleColorCodeStr + formats + text);
                    }
                    else if (count >= 2)
                    {
                        codeStr = motd.Substring(lastIndex, count);

                        FormattingCode format = FormattingCode.GetTypeFromCode(codeStr);
                        if (format != null)
                        {
                            if (format == FormattingCode.Reset)
                            {
                                formattingCodesStr.Clear();
                                lastSingleColorCodeStr = "";
                            }
                            else
                            {
                                formattingCodesStr.Add(codeStr);
                            }
                        }
                        //如果不是FormattingCode
                        else
                        {
                            lastSingleColorCodeStr = codeStr;
                        }

                    }
                }

                lastIndex = index;
            }

            //===============
            //将最后一组字符串增加到strs
            lastIndex = motd.LastIndexOf("§");
            subStr = motd.Substring(lastIndex);
            codeStr = subStr.Substring(0, 2);
            text = subStr.Substring(2);

            FormattingCode _format = FormattingCode.GetTypeFromCode(codeStr);
            string _formats = "";
            if (_format != null)
            {
                if (_format == FormattingCode.Reset)
                {
                    formattingCodesStr.Clear();
                    lastSingleColorCodeStr = "";
                }
                else
                {
                    formattingCodesStr.Add(codeStr);
                    foreach (string str in formattingCodesStr)
                    {
                        _formats += str;
                    }
                }
            }
            else
            {
                lastSingleColorCodeStr = codeStr;
            }

            ret.Add(lastSingleColorCodeStr + _formats + text);

            return ret;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            server = e.Parameter as Server;
            if (server.status != null)
            {
                if (server.status.players.sample != null)
                {          
                    sample = new ObservableCollection<Player>();

                    foreach (Player player in server.status.players.sample)
                    {
                        player.face = new BitmapImage(new Uri("ms-appx:///Assets/steve-32x32.png"));
                        /*
                        WriteableBitmap face = await SkinHelper.GetPlayerFaceAsync(player.name);
                        if (face != null)
                        {
                            player.face = face;
                        }
                        */
                        sample.Add(player);
                    }
                                  
                    if (sample.Count < server.status.players.online)
                    {
                        RemainPlayersMsgVisibility = Visibility.Visible;
                        RemainPlayersMsg = "剩余" + (server.status.players.online - sample.Count) + "位玩家";
                    }
                }
                else
                {
                    pivot.Items.Remove(pivotPlayer);
                }

                if (server.status.modinfo == null)
                {
                    pivot.Items.Remove(pivotMod);
                }

                if (!string.IsNullOrEmpty(server.status.description.text))
                {
                    ParseAndShowMotd(server.status.description.text);
                }
            }
   
        }
    }
}
