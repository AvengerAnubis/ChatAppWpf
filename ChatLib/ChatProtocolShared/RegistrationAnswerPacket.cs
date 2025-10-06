using ChatLib.NetShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLib.ChatProtocolShared
{
    public enum RegistrationStatus
    {
        NameExist, Failure, Success 
    }
    public class RegistrationAnswerPacket : PacketBase
    {
        public RegistrationStatus RegistrationStatus { get; set; }
    }
}
