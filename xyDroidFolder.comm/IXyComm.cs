using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public interface IXyComm
    {
        void start(Dictionary<string, string> pars);
        void send(string sendData);
        void send(Dictionary<string, string> sendDic);
        void sendStream(byte[] sendBytes, int sendLength);
        void startChatListen();
        void startStreamListen();
        void setTargetEndPoint(Dictionary<string, string> pEndPars);
        void setCommEventHandler(
            EventHandler<XyCommEventArgs> XyCommEventHandler
            );
        void clean();
    }
    public enum XyCommSendParKey
    {
        PassiveRegist,
        ActiveGetInitFolder,
        ActiveGetFolder,
        ActiveGetFile
    }
}
