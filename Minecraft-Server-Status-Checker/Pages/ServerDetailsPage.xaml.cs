using Lumia.Imaging;
using Lumia.Imaging.Extras.ImageSources;
using Minecraft_Server_Status_Checker.Status;
using Minecraft_Server_Status_Checker.Status.Motd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
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

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (server.status != null)
            {
                if (server.status.players != null && server.status.players.sample != null)
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
                        }*/

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

                MotdTextBlock.Content = server.status.description;

            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            server = e.Parameter as Server;
            
        }

    }

}
