using GoodTimeStudio.ServerPinger;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minecraft_Server_Status_Checker
{
    public class Server
    {
        public ImageSource ServerLogo;
        public string ServerName;
        public string ServerAddress;
        public string ServerPlayers;
        public int port;
        public string DisplayAddress;
        public string Motd;

        public Server(string ServerName, string ServerAddress, int port)
        {
            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.port = port;
            DisplayAddress = ServerAddress + ":" + port;
            ServerPlayers = "正在获取...";
            ServerLogo = new BitmapImage(new Uri("ms-appx:///Assets/ServerLogo.png"));

            GetServerInfo();
        }

        public async void GetServerInfo()
        {
            ServerPinger ping = new ServerPinger(ServerName, ServerAddress, port, PingVersion.MC_Current);
            ServerStatus status = await ping.GetStatus();
            
            if (status != null)
            {
                SetServerPlayers(status.players.online, status.players.max);
                if (!string.IsNullOrEmpty(status.favicon))
                {
                    var icon = status.favicon;
                    ServerLogo = await Base64ToImage(icon.Substring(icon.LastIndexOf(',') + 1));
                }
            }
        }

        public void SetServerPlayers(int online, int max)
        {
            ServerPlayers = online + "/" + max;
        }

        public async  static Task<BitmapImage> Base64ToImage(string strimage)
        {
            try
            {
                byte[] bitmapArray;
                bitmapArray = Convert.FromBase64String(strimage);
                MemoryStream ms = new MemoryStream(bitmapArray);
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                //将randomAccessStream 转成 IOutputStream
                var outputstream = randomAccessStream.GetOutputStreamAt(0);
                //实例化一个DataWriter
                DataWriter datawriter = new DataWriter(outputstream);
                //将Byte数组数据写进OutputStream
                datawriter.WriteBytes(bitmapArray);
                //在缓冲区提交数据到一个存储区
                await datawriter.StoreAsync();

                //将InMemoryRandomAccessStream给位图
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(randomAccessStream);

                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }

    }

}
