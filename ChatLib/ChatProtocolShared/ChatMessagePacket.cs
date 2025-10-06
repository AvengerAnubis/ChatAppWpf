using ChatLib.NetShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.ChatProtocolShared
{
    /// <summary>
    /// Базовый класс сообщения (текст сообщения, без ника и доп. данных)
    /// </summary>
    public class ChatMessagePacket : PacketBase
    {
        public string Message { get; set; } = string.Empty;
    }
}
