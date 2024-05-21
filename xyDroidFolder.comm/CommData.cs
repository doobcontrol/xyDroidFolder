using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class CommData
    {
        static public string commDicKey_cmdID = "cmdID";
        static public string commDicKey_cmd = "cmd";
        static public string commDicKey_cmdSucceed = "cmdSucceed";

        public CommData(XyPtoPCmd cmd) {
            this.cmd = cmd.ToString();
        }
        private CommData(Dictionary<string, string> parsDic)
        {

        }

        public string cmdID = Guid.NewGuid().ToString("N");
        public string cmd;
        public Dictionary<string, string> cmdParDic 
            = new Dictionary<string, string>();

        public Dictionary<string, string> toCommDic()
        {
            Dictionary<string, string> commDic
            = cmdParDic.ToDictionary<string,string>();

            commDic.Add(commDicKey_cmdID, cmdID);
            commDic.Add(commDicKey_cmd, cmd);

            return commDic;
        }

        public static CommData fromReceivedData(Dictionary<string, string> parsDic)
        {
            return new CommData(parsDic);
        }
    }
}
