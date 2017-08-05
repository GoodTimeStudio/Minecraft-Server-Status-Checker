using Minecraft_Server_Status_Checker.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minecraft_Server_Status_Checker
{
    public class Server : INotifyPropertyChanged
    {
        public string ServerName;
        public string ServerAddress;
        public int port;
        public ServerVersion version;

        public string DisplayAddress;

        private ServerStatus __status;
        public ServerStatus status
        {
            get { return __status; }
            set
            {
                if (__status != value)
                {
                    __status = value;
                    OnPropertyChanged();
                }
            }
        }

        private ImageSource __ServerLogo;
        public ImageSource ServerLogo
        {
            get { return __ServerLogo; }
            set
            {
                __ServerLogo = value;
                OnPropertyChanged();
            }
        }

        private string __DisplayServerPlayers;
        public string DisplayServerPlayers
        {
            get { return __DisplayServerPlayers; }
            set
            {
                if (__DisplayServerPlayers == null || !__DisplayServerPlayers.Equals(value))
                {
                    __DisplayServerPlayers = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility __DisplayServerPlayersVisable = Visibility.Collapsed;
        public Visibility DisplayServerPlayersVisable
        {
            get { return __DisplayServerPlayersVisable; }
            set
            {
                if (__DisplayServerPlayersVisable != value)
                {
                    __DisplayServerPlayersVisable = value;
                    OnPropertyChanged();
                }
            }
        }

        private string __DisplayMassage;
        public string DisplayMassage
        {
            get { return __DisplayMassage; }
            set
            {
                if (__DisplayMassage == null || !__DisplayMassage.Equals(value))
                {
                    __DisplayMassage = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility __DisplayMassageVisable;
        public Visibility DisplayMassageVisable
        {
            get { return __DisplayMassageVisable; }
            set
            {
                if (__DisplayMassageVisable != value)
                {
                    __DisplayMassageVisable = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Server(string ServerName, string ServerAddress, int port, ServerVersion version)
        {
            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.port = port;
            this.version = version;

            DisplayAddress = ServerAddress;
            if (port != 25565)
            {
                DisplayAddress += ":" + port;
            }

            DisplayMassage = "正在获取...";
            ServerLogo = new BitmapImage(new Uri("ms-appx:///Assets/ServerLogo.png"));
        }

        public async Task GetServerStatusAsync()
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

        }

        public void SetServerPlayers(int online, int max)
        {
            DisplayServerPlayers = online + "/" + max;
        }
       

    }

}
