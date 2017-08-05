using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Minecraft_Server_Status_Checker
{
    public class CoreManager
    {

        public static void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        #region config
        internal static async Task<ObservableCollection<Server>> LoadServersList()
        {
            ObservableCollection<Server> ret = new ObservableCollection<Server>();

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await folder.GetFileAsync("servers.json");
                string json = await FileIO.ReadTextAsync(file);

                JsonObject obj = JsonObject.Parse(json);
                JsonArray array = obj["servers"].GetArray();

                foreach (JsonValue value in array)
                {
                    JsonObject SubObj = value.GetObject();
                    Server server = new Server(SubObj["Name"].GetString(), SubObj["Address"].GetString(), int.Parse(SubObj["Port"].GetString()), Status.ServerVersion.MC_Current);
                    ret.Add(server);
                }

                return ret;

            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception)
            {
                StorageFile file = await folder.GetFileAsync("servers.json");
                await file.DeleteAsync();
            }

            return null;
        }

        internal static async Task SaveServersList(ObservableCollection<Server> Servers)
        {
            JsonObject obj = new JsonObject();
            JsonArray array = new JsonArray();

            foreach (Server ser in Servers)
            {
                JsonObject SubObj = new JsonObject();
                SubObj["Name"] = JsonValue.CreateStringValue(ser.ServerName);
                SubObj["Address"] = JsonValue.CreateStringValue(ser.ServerAddress);
                SubObj["Port"] = JsonValue.CreateStringValue(ser.port.ToString());

                array.Add(SubObj);
            }

            obj["servers"] = array;

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("servers.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, obj.ToString());
        }
        #endregion
    }

}
