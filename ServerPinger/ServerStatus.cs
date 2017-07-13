using Newtonsoft.Json;

namespace Minecraft_Server_Status_Checker.Status
{
    /*
     * http://wiki.vg/Server_List_Ping
     */
    [JsonObject]
    public class ServerStatus
    {
        
        [JsonIgnore]public string ServerName;
        [JsonIgnore]public string ServerAddress;
        [JsonIgnore]public int ServerPort;
        [JsonIgnore]public ServerVersion ServerVersion;

        public ServerVersionDescription version;
        public ServerPlayersInfo players;
        public Description description;
        public string favicon; //iamge base64 code
        public ModInfo modinfo;
        
    }  
}
