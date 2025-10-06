using ChatLib.NetShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib
{
    public class HostUdpService : UdpService
    {
        public HostUdpService() : base() { Port = 25500; }
    }
}
