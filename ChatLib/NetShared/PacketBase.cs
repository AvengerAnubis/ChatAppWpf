using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatLib.NetShared
{
    public abstract class PacketBase
    {
        public static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = false
        };

        //Базовые свойства
        /// <summary>
        /// Тип пакета, он же является полным именем типа класса
        /// </summary>
        public string? PacketTypeFullName
            => GetType().FullName;
        /// <summary>
        /// Версия библиотеки.
        /// Для сверки версий библиотек ChatLib отправителя и получателя.
        /// При этом версии клиентских приложения (GUI и т.д.) не сверяются,
        /// значит несоответствие приложений не влияет на общение между клиентами.
        /// Это позволяет взаимодействовать двум разным клиентам,
        /// например GUI и командной консоли
        /// </summary>
        public Version? LibVersion
            => Assembly.GetAssembly(GetType())?.GetName().Version;

        //Некоторые доп. данные пакета, не нуждающиеся в сериализации
        [JsonIgnore]
        public IPEndPoint? SenderEndPoint { get; set; }
        //JSON Сериализация
        public string JsonDataString
            => JsonSerializer.Serialize(this);
        public byte[] JsonDataBytes
            => Encoding.UTF8.GetBytes(JsonDataString);
        //JSON Десериализация
        public static T? FromJson<T>(string json)
            where T : PacketBase
            => JsonSerializer.Deserialize<T>(json, DefaultJsonOptions);
        public static PacketBase? FromBytes(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(json);
            }
            catch (Exception)
            {
                return null;
            }

            if (!doc.RootElement.TryGetProperty(nameof(PacketTypeFullName), out var typeProp))
                return null;

            string? typeName = typeProp.GetString();
            if (typeName is null)
                return null;

            // Проверка корретности типа объекта
            Type? type = Type.GetType(typeName);
            if (type is null)
                return null;
            if (!type.IsAssignableFrom(typeof(PacketBase)))
                return null;
            if (type.Assembly != typeof(PacketBase).Assembly)
                return null;

            if (JsonSerializer.Deserialize(json, type, DefaultJsonOptions) is PacketBase packetBase)
                return packetBase;
            return null;
        }
    }
}
