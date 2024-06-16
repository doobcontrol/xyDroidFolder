using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class CommResult
    {
        public CommResult(CommData receivedData)
        {
            cmdID = receivedData.cmdID;
        }
        private CommResult(string ReturnString)
        {
            string[] pars = ReturnString.Split(',');
            Dictionary<string, string> resultDic =
                new Dictionary<string, string>();
            foreach (string par in pars)
            {
                string[] parArr = par.Split('=');
                if (parArr.Length != 2) { 
                    throw new DroidFolderCommException(
                        DroidFolderCommErrorCode.OtherError,
                        "ReturnString: " + ReturnString); 
                }
                resultDic.Add(parArr[0], parArr[1]);
            }

            this.cmdID = resultDic[CmdPar.cmdID.ToString()];
            this.cmdSucceed = bool.Parse(resultDic[CmdPar.cmdSucceed.ToString()]);

            this.resultDataDic
                = resultDic.ToDictionary<string, string>();
            this.resultDataDic.Remove(CmdPar.cmdID.ToString());
            this.resultDataDic.Remove(CmdPar.cmdSucceed.ToString());
        }

        public bool cmdSucceed = true;
        public bool errorCmdID = false;
        public string cmdID;
        public Dictionary<string, string> resultDataDic
            = new Dictionary<string, string>();

        public string toSendString()
        {
            string sendString = CmdPar.cmdID + "=" + cmdID;
            sendString += "," + CmdPar.cmdSucceed + "=" + cmdSucceed;
            foreach (string pName in resultDataDic.Keys)
            {
                sendString += "," + pName + "=" + resultDataDic[pName];
            }
            return sendString;
        }

        public static CommResult fromReturnString(
            string ReturnString,
            CommData sendData
            )
        {
            CommResult commResult = new CommResult(ReturnString);
            commResult.errorCmdID = (commResult.cmdID != sendData.cmdID);
            return new CommResult(ReturnString);
        }
    }
}
