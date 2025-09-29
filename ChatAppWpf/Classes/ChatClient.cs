using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ChatAppWpf
{
    internal class ChatClient
    {
        private static ChatClient? _instance;
        public static ChatClient Instance
        {
            get
            {
                _instance ??= new ChatClient();
                return _instance;
            }
        }

        private bool _isRunning = false;
        private UdpClient _client;
        private IPEndPoint? _endPoint;

        public bool Connect(string ip, int port)
        {
            Ping ping = new Ping();
            PingReply reply = ping.Send(ip, port);
            if (reply.Status == IPStatus.Success)
            {
                _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                _client = new UdpClient(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
                _isRunning = true;
                BeginListening();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Close()
        {
            _isRunning = false;
            _client.Close();
        }

        public delegate void MessageReceivedEventHandler(string msg);
        public event MessageReceivedEventHandler? MessageReceived;
        private async Task BeginListening()
        {
            while (_isRunning)
            {
                byte[] receiveBytes = (await _client.ReceiveAsync()).Buffer;

                string returnData = Encoding.UTF8.GetString(receiveBytes);

                MessageReceived?.Invoke(returnData);
            }
        }

        public void SendMessage(string message)
        {
            if (_endPoint != null && _isRunning)
            {
                byte[] sendBytes = Encoding.UTF8.GetBytes(message);

                _client.Send(sendBytes, sendBytes.Length, _endPoint);
            }
        }
    }
}
