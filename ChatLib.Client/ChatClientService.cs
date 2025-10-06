using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatLib.ChatProtocolShared;
using ChatLib.NetShared;

namespace ChatLib
{
    public enum PeerToPeerConnectionStatus
    {
        Success, Timeout, Failure
    }
    public class ChatClientSettings
    {
        public string Name { get; set; } = $"user{DateTime.Now.Ticks}";
    }

    public class ChatClientService(ClientUdpService udpService)
    {
        protected readonly CancellationTokenSource connectPeerToPeerCts = new();
        protected readonly ChatClientSettings settings = new();
        public void Configure(string name)
        {
            settings.Name = name;
        }

        public async Task<PeerToPeerConnectionStatus> ConnectPeerToPeer
            (IPEndPoint peer, CancellationToken? token = null)
        {
            // Позволяем вызывающему отменять действие прослушки пакетов
            token?.Register(() => connectPeerToPeerCts.Cancel());

            await udpService.SendPacketAsync(new RegistrationRequestPacket()
            {
                ConnectionMode = ChatConnectionMode.PeerToPeer,
                Name = settings.Name
            }, peer);

        }
    }
}
