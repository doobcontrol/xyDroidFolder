using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class CommResult
    {
        static public string resultDataKey_ActiveEndInfo = "ActiveEndInfo";
        static public string resultDataKey_ErrorInfo = "ErrorInfo";
        static public string resultDataKey_cmdSucceed = "cmdSucceed";

        public CommResult(CommData receivedData)
        {
            cmdID = receivedData.cmdID;
        }
        private CommResult(Dictionary<string, string> resultDic)
        {
            this.cmdID = resultDic[CommData.commDicKey_cmdID];
            this.cmdSucceed = bool.Parse(resultDic[resultDataKey_cmdSucceed]);

            this.resultDataDic
                = resultDic.ToDictionary<string, string>();
            this.resultDataDic.Remove(CommData.commDicKey_cmdID);
        }

        public bool cmdSucceed;
        public bool errorCmdID = false;
        public string cmdID;
        public Dictionary<string, string> resultDataDic
            = new Dictionary<string, string>();

        public Dictionary<string, string> toCommDic()
        {
            Dictionary<string, string> commDic
            = resultDataDic.ToDictionary<string, string>();

            commDic.Add(CommData.commDicKey_cmdID, cmdID);
            commDic.Add(resultDataKey_cmdSucceed, cmdSucceed.ToString());

            return commDic;
        }

        public static CommResult fromReceivedResult(Dictionary<string, string> parsDic)
        {
            return new CommResult(parsDic);
        }
    }
}
