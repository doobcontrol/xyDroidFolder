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
        void sendFile(string sendFile);
        void startListen();
        void setTargetEndPoint(Dictionary<string, string> pEndPars);
        void setCommEventHandler(
            EventHandler<XyCommEventArgs> XyCommEventHandler
            );
    }
}
