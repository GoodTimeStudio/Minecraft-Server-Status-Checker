using Minecraft_Server_Status_Checker.Status;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minecraft_Server_Status_Checker
{
    public class Server
    {
        public string ServerName;
        public string ServerAddress;
        public int port;
        public ServerVersion version;

        public ServerStatus status;

        public ImageSource ServerLogo;
        public string DisplayServerPlayers;  
        public string DisplayAddress;
        public Visibility ProgressVisable = Visibility.Visible;
        public Visibility DisplayServerPlayersVisable = Visibility.Collapsed;
        public string DisplayMassage;
        public Visibility DisplayMassageVisable;

        public Server(string ServerName, string ServerAddress, int port, ServerVersion version)
        {
            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.port = port;
            this.version = version;

            DisplayAddress = ServerAddress + ":" + port;
            DisplayMassage = "正在获取...";
            ServerLogo = new BitmapImage(new Uri("ms-appx:///Assets/ServerLogo.png"));

            GetServerStatus();       
        }

        public async void GetServerStatus()
        {
            ServerPinger ping = new ServerPinger(ServerName, ServerAddress, port, version);
            ServerStatus status = await ping.GetStatus();
            
            if (status != null)
            {
                this.status = status;

                if (status.players != null)
                    SetServerPlayers(status.players.online, status.players.max);

                if (!string.IsNullOrEmpty(status.favicon))
                {
                    var icon = status.favicon;
                    ServerLogo = await Base64Helper.Base64ToImage(icon.Substring(icon.LastIndexOf(',') + 1));
                }

                DisplayMassageVisable = Visibility.Collapsed;
                DisplayServerPlayersVisable = Visibility.Visible;
            }
            else
            {
                DisplayMassage = "获取服务器信息失败";
            }

            ProgressVisable = Visibility.Collapsed;
        }

        public void SetServerPlayers(int online, int max)
        {
            DisplayServerPlayers = online + "/" + max;
        }
       

    }

}
