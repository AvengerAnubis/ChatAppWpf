using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.NetShared
{
	public class UdpService : IDisposable
	{
		protected UdpClient udpClient = new();
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

        CancellationTokenSource getUdpPacketsCts = new CancellationTokenSource();
		/// <summary>
		/// Асинхронно возвращает полученные пакеты в порядке их поступления
		/// </summary>
		/// <returns>Пакеты, полученные по UDP, 
		/// тип которых определяется по свойству <see cref="PacketBase.PacketTypeFullName"/></returns>
		public async IAsyncEnumerable<PacketBase> GetUdpPackets()
		{
			while (true)
			{
				UdpReceiveResult result;
				try
				{
					// Получаем датаграмму асинхронно
					result = await udpClient.ReceiveAsync(getUdpPacketsCts.Token);
				}
				// Отслеживаем отмену операции
				catch (OperationCanceledException) 
				{ 
					yield break;
				}
				// Получаем объект пакета из датаграммы
				PacketBase? packet = PacketBase.FromBytes(result.Buffer);
				if (packet is null)
					continue;
				//Сохраняем отправителя
				packet.SenderEndPoint = result.RemoteEndPoint;

				yield return packet;
			}
		}

		/// <summary>
		/// Освобождение ресурсов
		/// </summary>
		public void Dispose()
		{
			// Закрываем прослушку портов
            if (!getUdpPacketsCts.IsCancellationRequested)
                getUdpPacketsCts.Cancel();
			// Закрываем UDP клиент
            udpClient.Close();
            udpClient.Dispose();
        }
	}
}
