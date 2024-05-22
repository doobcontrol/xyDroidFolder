using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class XyPtoPEnd
    {
        private IXyComm myIXyComm;
        private XyPtoPEndType myXyPtoPEndType;
        private XyPtoPRequestHandler _xyPtoPRequestHandler;
        public XyPtoPEnd(
            XyPtoPEndType endType, 
            Dictionary<string, string> pEndPars,
            XyPtoPRequestHandler xyPtoPRequestHandler
            )
        {
            myXyPtoPEndType = endType;
            _xyPtoPRequestHandler = xyPtoPRequestHandler;

            myIXyComm = new XyUdpComm(pEndPars); //配置实现类？
            myIXyComm.setCommEventHandler(myIXyCommEventRaise);

            myIXyComm.startListen();
        }

        private void myIXyCommEventRaise(object sender, XyCommEventArgs e)
        {
            string receivedString =
                Encoding.UTF8.GetString(
                    e.ReceivedBytes, 0, e.ReceivedBytes.Length);

            string[] pars = receivedString.Split(',');
            Dictionary<string, string> parsDic = new Dictionary<string, string>();
            foreach (string par in pars)
            {
                string[] parArr = par.Split('=');
                parsDic.Add(parArr[0], parArr[1]);
            }

            if (parsDic.ContainsKey(CommData.commDicKey_cmd))
            {
                //cmd send by other end. need reply
                CommData commData = CommData.fromReceivedData(parsDic);

                CommResult commResult = new CommResult(commData);
                switch (commData.cmd)
                {
                    //cmd from PassiveEnd
                    case XyPtoPCmd.PassiveRegist:

                        myIXyComm.setTargetEndPoint(commData.cmdParDic);

                        _xyPtoPRequestHandler(commData, commResult);

                        commResult.resultDataDic.Add(
                            CommResult.resultDataKey_ActiveEndInfo, 
                            "Regist ok");
                        commResult.cmdSucceed = true;
                        break;

                    //cmd from ActiveEnd
                    case XyPtoPCmd.ActiveGetInitFolder:
                        _xyPtoPRequestHandler(commData, commResult);
                        commResult.cmdSucceed = true;
                        break;

                    default:
                        commResult.resultDataDic.Add(
                            CommResult.resultDataKey_ErrorInfo, 
                            "Cannot hand this command");
                        commResult.cmdSucceed = true;
                        break;
                }

                sendResult(commResult);
            }
            else
            {
                //is Reply. set sendDataResult to not sendData function
                sendDataResult = CommResult.fromReceivedResult(parsDic);
            }
        }
        public void clean()
        {
            if (myIXyComm != null)
            {
                myIXyComm.clean();
            }
        }

        #region 命令

        private int sleepTime = 50;
        private CommResult sendDataResult;
        public async Task<CommResult> sendData(
            CommData commData
            )
        {
            CommResult taskResult =  await Task.Run(
                () => {
                    sendDataResult = null;
                    myIXyComm.send(commData.toCommDic());

                    while (true)
                    {
                        if(sendDataResult != null)
                        {
                            //检查是否配置的响应                            
                            if(sendDataResult.cmdID != commData.cmdID)
                            {
                                sendDataResult.errorCmdID = true;
                            }

                            break;
                        }

                        //超时检查

                        Thread.Sleep(sleepTime);
                    }

                    return sendDataResult;
                });

            return taskResult;
        }

        private void sendResult(CommResult commResult)
        {
            myIXyComm.send(commResult.toCommDic());
        }


        #region 主控端命令

        static public string FolderparKey_folders = "folders";
        static public string FolderparKey_files = "files";
        static public string FolderparKey_hostName = "hostName";
        public async Task<CommResult> ActiveGetInitFolder()
        {
            CommData commData = new CommData(XyPtoPCmd.ActiveGetInitFolder);

            return await sendData(commData);
        }

        #endregion

        #region 被控端命令

        public async Task<CommResult> Regist(
            Dictionary<string, string> pLocalEndPars,
            Dictionary<string, string> pRemoteEndPars
            )
        {
            myIXyComm.setTargetEndPoint(pRemoteEndPars);

            CommData commData = new CommData(XyPtoPCmd.PassiveRegist);
            commData.cmdParDic = pLocalEndPars;

            return await sendData(commData);
        }

        #endregion

        #endregion
    }

    public delegate void
        XyPtoPRequestHandler(CommData commData, CommResult commResult);
    public enum XyPtoPEndType { ActiveEnd, PassiveEnd }
    public enum XyPtoPCmd {
        PassiveRegist,
        ActiveGetInitFolder
    }
}
