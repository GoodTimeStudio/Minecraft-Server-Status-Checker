using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using System.IO;
using Newtonsoft.Json;

#if DEBUG
using System.Diagnostics;
#endif

namespace GoodTimeStudio.ServerPinger
{

    public class ServerPinger
    {
        public int timeout = 8000;

        public string ServerName;
        public string ServerAddress;
        public int ServerPort;
        public PingVersion ServerVersion;

        public ServerPinger(string ServerName, string ServerAddress,
            int ServerPort, PingVersion ServerVersion)
        {
            this.ServerName = ServerName;
            this.ServerAddress = ServerAddress;
            this.ServerPort = ServerPort;
            this.ServerVersion = ServerVersion;
        }

        public async Task<ServerStatus> GetStatus()
        {
            try {
                if (ServerVersion == PingVersion.MC_Current)
                {
                    return await GetStatusCurrent();
                }
                return null;
            }
            catch
            {
                return null;
            }           
        }

        private async Task<ServerStatus> GetStatusCurrent()
        {
            try
            {
                using (StreamSocket socket = new StreamSocket())
                {
                    await socket.ConnectAsync(new HostName(ServerAddress), ServerPort.ToString());
                    BinaryWriter writer;

                    #region handshake
#if DEBUG
                    Debug.WriteLine("Sending handshake packet");
#endif
                    MemoryStream handshakeStream = new MemoryStream();
                    BinaryWriter handshakewriter = new BinaryWriter(handshakeStream);

                    handshakewriter.Write((byte)0x00);  // Packet ID
                    // Protocol version, http://wiki.vg/Protocol_version_numbers
                    handshakewriter.Write(VarintHelper.IntToVarint(210));
                    handshakewriter.Write(GetByteFromString(ServerAddress)); // hostname or IP
                    handshakewriter.Write((short)ServerPort); // Port
                    handshakewriter.Write(VarintHelper.IntToVarint(0x01)); // Next state, 1 for `status'
                    handshakewriter.Flush();

                    writer = new BinaryWriter(socket.OutputStream.AsStreamForWrite());
                    writer.Write(VarintHelper.IntToVarint((int)handshakeStream.Length));
                    writer.Write(handshakeStream.ToArray());
                    writer.Flush();
                    #endregion

                    writer = new BinaryWriter(socket.OutputStream.AsStreamForWrite());
                    /* BE: 0x0100, Length and writer.Write((byte)0x00);
                     * ID for `Request'
                     */
                    writer.Write((short)0x0001);
                    writer.Flush();

#if DEBUG
                    Debug.WriteLine("Pinging");
#endif
                    var streamIn = socket.InputStream;
                    BinaryReader reader = new BinaryReader(streamIn.AsStreamForRead());
                    var packetLen = VarintHelper.ReadVarInt(reader);
                    var packetId = VarintHelper.ReadVarInt(reader);
                    var packetJsonLen = VarintHelper.ReadVarInt(reader);
                    var response = reader.ReadBytes(packetJsonLen);
                    string json = Encoding.UTF8.GetString(response);
#if DEBUG
                    Debug.WriteLine("json:" + json);
#endif

#if DEBUG
                    Debug.WriteLine("Parsing response json");
#endif
                    return JsonConvert.DeserializeObject<ServerStatus>(json);
                }
            }
            catch
            {
                return null;
            }
            
        } 

        private byte[] GetByteFromString(string content)
        {
            List<byte> output = new List<byte>();

            output.AddRange(VarintHelper.IntToVarint(content.Length));
            output.AddRange(Encoding.UTF8.GetBytes(content));

            return output.ToArray();
        }
    }
}
