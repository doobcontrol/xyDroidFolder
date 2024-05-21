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

        public XyPtoPEnd(
            XyPtoPEndType endType, 
            Dictionary<string, string> pEndPars
            )
        {
            myXyPtoPEndType = endType;

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

                XyPtoPCmd cmd = 
                    (XyPtoPCmd)Enum.Parse(
                        typeof(XyPtoPCmd), 
                        parsDic[CommData.commDicKey_cmd], 
                        false
                        );

                switch (cmd)
                {
                    case XyPtoPCmd.PassiveRegist:

                        commResult.resultDataDic.Add(
                            CommResult.resultDataKey_ActiveEndInfo, 
                            ""
                            );
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

        #region 命令

        private int sleepTime = 50;
        private CommResult sendDataResult;
        public async Task<CommResult> sendData(CommData commData)
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

        #endregion

        #region 被控端命令

        public async Task<CommResult> Regist(Dictionary<string, string> pEndPars)
        {
            CommData commData = new CommData(XyPtoPCmd.PassiveRegist);

            return await sendData(commData);
        }

        #endregion

        #endregion
    }
    public enum XyPtoPEndType { ActiveEnd, PassiveEnd }
    public enum XyPtoPCmd {
        PassiveRegist, 
    }
}
