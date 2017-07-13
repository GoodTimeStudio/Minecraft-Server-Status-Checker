using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Minecraft_Server_Status_Checker.Status
{
    public class Base64Helper
    {
        public static string DecodeBase64(string base64)
        {
            byte[] tmp = Convert.FromBase64String(base64);
            string ret = Encoding.UTF8.GetString(tmp);
            return ret;
        }

        public static string EncodeBase64(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public async static Task<BitmapImage> Base64ToImage(string strimage)
        {
            try
            {
                byte[] bitmapArray;
                bitmapArray = Convert.FromBase64String(strimage);
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();

                DataWriter datawriter = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
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
