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

        public CommResult(CommData receivedData)
        {
            cmdID = receivedData.cmdID;
        }
        private CommResult(Dictionary<string, string> resultDic) { 
        
        }

        public bool cmdSucceed;
        public string cmdID;
        public Dictionary<string, string> resultDataDic
            = new Dictionary<string, string>();

        public Dictionary<string, string> toCommDic()
        {
            Dictionary<string, string> commDic
            = resultDataDic.ToDictionary<string, string>();

            commDic.Add(CommData.commDicKey_cmdID, cmdID);
            commDic.Add(CommData.commDicKey_cmdSucceed, cmdSucceed.ToString());

            return commDic;
        }

        public static CommResult fromReceivedResult(Dictionary<string, string> parsDic)
        {
            return new CommResult(parsDic);
        }
    }
}
