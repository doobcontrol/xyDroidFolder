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

        public CommData(XyPtoPCmd cmd) {
            this.cmd = cmd;
        }
        private CommData(Dictionary<string, string> parsDic)
        {
            this.cmd = (XyPtoPCmd)Enum.Parse(
                typeof(XyPtoPCmd), parsDic[commDicKey_cmd], false);
            this.cmdID = parsDic[commDicKey_cmdID];

            this.cmdParDic
                = parsDic.ToDictionary<string, string>();
            this.cmdParDic.Remove(commDicKey_cmd);
            this.cmdParDic.Remove(cmdID);
        }

        public string cmdID = Guid.NewGuid().ToString("N");
        public XyPtoPCmd cmd;
        public Dictionary<string, string> cmdParDic 
            = new Dictionary<string, string>();

        public Dictionary<string, string> toCommDic()
        {
            Dictionary<string, string> commDic
            = cmdParDic.ToDictionary<string,string>();

            commDic.Add(commDicKey_cmdID, cmdID);
            commDic.Add(commDicKey_cmd, cmd.ToString());

            return commDic;
        }

        public static CommData fromReceivedData(Dictionary<string, string> parsDic)
        {
            return new CommData(parsDic);
        }
    }
}
