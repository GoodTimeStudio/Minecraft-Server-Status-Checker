using System.Collections.Generic;

namespace GoodTimeStudio.ServerPinger
{
    public class Player
    {
        public string name;
        public string uuid;

        public Player(string name, string uuid)
        {
            this.name = name;
            this.uuid = uuid;
        }
    }

    public class ServerPlayers
    {
        public int max;
        public int online;
        public List<Player> sample;
    }
}
