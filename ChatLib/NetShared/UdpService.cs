using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.NetShared
{
	public class UdpService : IDisposable
	{
        #region UdpClient
        protected readonly UdpClient udpClient = new();
        /// <summary>
        /// Получает или задает порт клиента
        /// </summary>
        public int Port
        {
            get
            {
                if (udpClient.Client.LocalEndPoint is IPEndPoint endPoint)
                    return endPoint.Port;
                else
                    return 0;
            }
            set
            {
                udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, value));
            }
        }
        #endregion

        #region Отправка пакетов
        /// <summary>
        /// Отправка пакета <paramref name="packet"/> на <paramref name="remoteEndPoint"/>
        /// </summary>
        /// <param name="packet">Пакет</param>
        /// <param name="remoteEndPoint">IP эндпоинт</param>
        /// <returns></returns>
        public async Task SendPacketAsync(PacketBase packet, IPEndPoint remoteEndPoint)
		{
			byte[] data = packet.JsonDataBytes;
			await udpClient.SendAsync(data, data.Length, remoteEndPoint);
		}
		/// <summary>
		/// Отправка пакета <paramref name="packet"/> на указанные <paramref name="ip"/> и <paramref name="port"/>
		/// </summary>
		/// <param name="packet">Пакет</param>
		/// <param name="ip">IP</param>
		/// <param name="port">Порт</param>
		/// <returns></returns>
		public async Task SendPacketAsync(PacketBase packet, string ip, int port)
		{
			var endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
			await SendPacketAsync(packet, endPoint);
		}

        /// <summary>
        /// Отправка пакета <paramref name="packet"/> на все эндпоинты из списка <paramref name="endPoints"/>
        /// </summary>
        /// <param name="packet">Пакет</param>
        /// <param name="endPoints">Эндпоинты, на которые будут рассылаться пакеты</param>
        /// <returns></returns>
        public async Task BroadcastPacketAsync(PacketBase packet, IEnumerable<IPEndPoint> endPoints)
        {
            foreach (var endPoint in endPoints)
            {
                await SendPacketAsync(packet, endPoint);
            }
        }
        #endregion

        #region Получение пакетов
		public event EventHandler<PacketBase>? PacketReceived;

        protected readonly CancellationTokenSource beginListeningCts = new();
		public async Task BeginListening(CancellationToken? token = null)
		{
            token?.Register(() => beginListeningCts.Cancel());
            while (!beginListeningCts.IsCancellationRequested)
            {
                UdpReceiveResult result;
                try
                {
                    // Получаем датаграмму асинхронно
                    result = await udpClient.ReceiveAsync(beginListeningCts.Token);
                }
                // Отслеживаем отмену операции
                catch (OperationCanceledException)
                {
                    break;
                }
                // Получаем объект пакета из датаграммы
                PacketBase? packet = PacketBase.FromBytes(result.Buffer);
                if (packet is null)
                    continue;
                //Сохраняем отправителя
                packet.SenderEndPoint = result.RemoteEndPoint;

                PacketReceived?.Invoke(this, packet);
            }
        }
        #endregion

        #region Освобождение ресурсов Dispose
        protected bool isDisposed = false;
		/// <summary>
		/// Освобождение ресурсов
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				isDisposed = true;
				if (disposing)
				{
				    // Закрываем прослушку портов
				    if (!beginListeningCts.IsCancellationRequested)
				        beginListeningCts.Cancel();
				    // Закрываем UDP клиент
				    udpClient.Close();
				    udpClient.Dispose();
				}
			}
		}
        #endregion
    }
}
