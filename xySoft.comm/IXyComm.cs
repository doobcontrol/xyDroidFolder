using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xySoft.comm
{
    public interface IXyComm
    {
        Task<string> sendForResponseAsync(string sendData);
        void startListen();
        void stopListen();
        void set(Dictionary<string, string> setDic);
        Task prepareStreamReceiver(
            string file, 
            string fileLength, 
            string streamReceiverPar,
            EventHandler<XyCommFileEventArgs> xyCommFileEventHandler);
        Task sendFile(
            string file, 
            string fileLength, 
            string streamReceiverPar,
            EventHandler<XyCommFileEventArgs> xyCommFileEventHandler);
    }

    public delegate string
        XyCommRequestHandler(string sendData);

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
