using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class CommData
    {
        public CommData(DroidFolderCmd cmd) {
            this.cmd = cmd;
        }
        private CommData(string receivedString)
        {
            string[] pars = receivedString.Split(',');
            Dictionary<string, string> parsDic = 
                new Dictionary<string, string>();
            foreach (string par in pars)
            {
                string[] parArr = par.Split('=');
                parsDic.Add(parArr[0], parArr[1]);
            }

            this.cmd = (DroidFolderCmd)Enum.Parse(
                typeof(DroidFolderCmd), parsDic[CmdPar.cmd.ToString()], false);
            this.cmdID = parsDic[CmdPar.cmdID.ToString()];

            this.cmdParDic
                = parsDic.ToDictionary<string, string>();
            this.cmdParDic.Remove(CmdPar.cmd.ToString());
            this.cmdParDic.Remove(cmdID);
        }

        public string cmdID = Guid.NewGuid().ToString("N");
        public DroidFolderCmd cmd;
        public Dictionary<string, string> cmdParDic 
            = new Dictionary<string, string>();

        public string toSendString()
        {
            string sendString = CmdPar.cmd + "=" + cmd;
            sendString += "," + CmdPar.cmdID + "=" + cmdID;
            foreach (string pName in cmdParDic.Keys)
            {
                sendString += "," + pName + "=" + cmdParDic[pName];
            }
            return sendString;
        }

        public static CommData fromReceivedString(string receivedString)
        {
            return new CommData(receivedString);
        }
    }
}
