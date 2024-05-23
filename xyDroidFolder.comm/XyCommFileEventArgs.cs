using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class XyCommFileEventArgs : EventArgs
    {
        public XyCommFileEventArgs(
            XyCommFileSendReceive fileSendReceive, 
            long length, 
            long progress)
        {
            this.FileSendReceive = fileSendReceive;
            Length = length;
            Progress = progress;
        }

        public XyCommFileSendReceive FileSendReceive { get; private set; }
        public long Length { get; private set; }
        public long Progress { get; private set; }
    }
    public enum XyCommFileSendReceive { Send, Receive }
}
