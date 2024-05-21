using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class XyCommEventArgs : EventArgs
    {
        public XyCommEventArgs(byte[] receivedBytes)
        {
            this.ReceivedBytes = receivedBytes;
        }
        public byte[] ReceivedBytes { get; private set; }
    }
}
