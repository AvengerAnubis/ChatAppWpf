using ChatLib.NetShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.ChatProtocolShared
{
    public enum ChatConnectionMode
    {
        PeerToPeer, Host
    }
    public class RegistrationRequestPacket : PacketBase
    {
        public string Name { get; set; } = string.Empty;
        public bool RequestChatHistory { get; set; } = false;
        public ChatConnectionMode ConnectionMode { get; set; } = ChatConnectionMode.Host;
    }
}
