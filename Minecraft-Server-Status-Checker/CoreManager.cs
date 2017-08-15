using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public async static Task InitApp()
        {
            MainPage.PreServers = await CoreManager.LoadServersList();

        }

        #region config
        internal static async Task<ServerListSerializer> LoadServersList()
        {
            List<Server> ret = new List<Server>(); 

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await folder.GetFileAsync("servers.json");
                string json = await FileIO.ReadTextAsync(file);

                return JsonConvert.DeserializeObject<ServerListSerializer>(json);

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

        internal static async Task SaveServersList(ServerListSerializer seriailizer)
        {
            if (seriailizer != null)
            {
                string obj = JsonConvert.SerializeObject(seriailizer);

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync("servers.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, obj.ToString());
            }
        }

        internal static async Task SaveServersList(IEnumerable<Server> ServerList, int SelectedServerIndex)
        {
            ServerListSerializer seriailizer = new ServerListSerializer();
            seriailizer.ServerList = new List<Server>();
            seriailizer.ServerList.AddRange(ServerList);
            seriailizer.SelectedServerIndex = SelectedServerIndex;

            await SaveServersList(seriailizer);
        }
        #endregion
    }

    public class ServerListSerializer
    {
        public List<Server> ServerList;
        public int SelectedServerIndex;
    }

}
