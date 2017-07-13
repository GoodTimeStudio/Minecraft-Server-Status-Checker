using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Server_Status_Checker
{
    class ServerStatusLoadedEvent
    {
        public delegate void ServerStatusLoadedHandler();

        public event ServerStatusLoadedHandler onLoaded;
    }
}
