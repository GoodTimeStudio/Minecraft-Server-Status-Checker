using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Minecraft_Server_Status_Checker.Status
{
    public enum ServerVersion
    {
        MC_Current,
        MC_16,
        MC_14_15,
        MC_Beta18_13,
        MC_PE
    }

    public class Mod
    {
        public string modid;
        public string version;
    }

    public class ServerVersionDescription
    {
        public string name;
        public int protocol;
    }


    public class Description
    {
        public string text;
        //TO-DO: extra (动态ping插件)
    }

    public class Player
    {
        [JsonIgnore] public ImageSource face = new BitmapImage(new Uri("ms-appx:///Assets/steve-32x32.png"));
        public string name;
        public string uuid;

        public Player(string name, string uuid)
        {
            this.name = name;
            this.uuid = uuid;
        }
    }

    public class ServerPlayersInfo
    {
        public int max;
        public int online;
        public List<Player> sample;
    }

    public class ModInfo
    {
        public string type;
        public List<Mod> modList;
    }

}
