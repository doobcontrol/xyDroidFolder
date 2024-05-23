using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class XyCommEventArgs : EventArgs
    {
        public XyCommEventArgs(
            byte[] receivedBytes,
            XyCommEventMsgType msgType)
        {
            this.ReceivedBytes = receivedBytes;
            this.MsgType = msgType;
        }
        public byte[] ReceivedBytes { get; private set; }
        public XyCommEventMsgType MsgType { get; private set; }
    }
    public enum XyCommEventMsgType { Chat, Stream }
}
