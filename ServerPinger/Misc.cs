namespace GoodTimeStudio.ServerPinger
{
    public enum PingVersion
    {
        MC_Current,
        MC_16,
        MC_14_15,
        MC_Beta18_13,
        MC_PE
    }

    public class Mod
    {
        public string ModName;
        public string ModVersion;

        public Mod(string ModName, string ModVersion)
        {
            this.ModName = ModName;
            this.ModVersion = ModVersion;
        }
    }

    public class ServerVersion
    {
        public string name;
        public int protocol;
    }


    public class Description
    {
        public string text;
        //TO-DO: extra (动态ping插件)
    }
}
