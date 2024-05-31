using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xySoft.comm;

namespace xyDroidFolder.comm
{
    public class DroidFolderComm
    {
        private IXyComm myIXyComm;
        private DroidFolderRequestHandler _droidFolderRequestHandler;

        public DroidFolderComm(
            string localIp, int localPort,
            string targetIp, int targetPort,
            DroidFolderRequestHandler droidFolderRequestHandler
            )
        {
            myIXyComm = new XyUdpComm(
                localIp, localPort,
                targetIp, targetPort,
                XyCommRequestHandler);
            _droidFolderRequestHandler = droidFolderRequestHandler;
            myIXyComm.startListen();
        }

        public async Task<CommResult> Register(
            string ip, int port, string hostName
            )
        {
            CommData commData = new CommData(DroidFolderCmd.Register);
            commData.cmdParDic.Add(CmdPar.ip.ToString(), ip);
            commData.cmdParDic.Add(CmdPar.port.ToString(), port.ToString());
            commData.cmdParDic.Add(CmdPar.hostName.ToString(), hostName);

            string resultString = await myIXyComm.sendForResponseAsync(
                commData.toSendString()
                );

            return CommResult.fromReturnString(resultString, commData);
        }

        private string XyCommRequestHandler(string receivedString)
        {
            CommData commData = CommData.fromReceivedString(receivedString);
            CommResult commResult = new CommResult(commData);

            switch (commData.cmd)
            {
                case DroidFolderCmd.Register:
                    //register target ip and port
                    Dictionary<string, string> setDic = new Dictionary<string, string>();
                    setDic[XyUdpCommTargetSetPar.ip.ToString()] 
                        = commData.cmdParDic[CmdPar.ip.ToString()];
                    setDic[XyUdpCommTargetSetPar.port.ToString()]
                        = commData.cmdParDic[CmdPar.port.ToString()];
                    myIXyComm.set(setDic);

                    _droidFolderRequestHandler(commData, commResult);

                    commResult.resultDataDic.Add(
                        CmdPar.returnMsg.ToString(),
                        "Regist ok");
                    break;
            }

            return commResult.toSendString();
        }
    }
    public enum DroidFolderCmd
    {
        Register,
        GetInitFolder,
        GetFolder,
        GetFile,
        SendFile
    }
    public enum CmdPar
    {
        cmd,
        cmdID,
        cmdSucceed,
        ip,
        port,
        hostName,
        returnMsg,
        errorMsg
    }

    public delegate void
        DroidFolderRequestHandler(CommData commData, CommResult commResult);
}
