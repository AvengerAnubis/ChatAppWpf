using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.Shared.NetShared
{
    public class QuicService
    {
        public QuicService()
        {
            // Перспективный протокол для чата, более безопасный, работает поверх UDP
            // Есть шифрование TLS 1.3 (самописные/подлинные сертификаты)
            // Но для простого чата пока что overkill
            throw new NotImplementedException();
        }
    }
}
