using Minecraft_Server_Status_Checker.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Server_Status_Checker
{
    public class Server
    {
        public string ServerName;
        public string ServerAddress;
        public int port;
        public ServerVersion version;

        public Server(string ServerName, string ServerAddress, int port, ServerVersion version)
        {
            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.port = port;
            this.version = version;
        }
    }
}
