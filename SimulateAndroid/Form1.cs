using System.IO;
using xyDroidFolder.comm;

namespace SimulateAndroid
{
    public partial class Form1 : Form
    {
        XyPtoPEnd xyPtoPEnd;

        public Form1()
        {
            InitializeComponent();
            Text = "SimulateAndroid";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "start regist ……";

            Dictionary<string, string> pLocalEndPars 
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_localIP, "192.168.168.129");
            pLocalEndPars.Add(XyUdpComm.workparKey_localChatPort, "12921");
            pLocalEndPars.Add(XyUdpComm.workparKey_localStreamPort, "12922");

            xyPtoPEnd = new XyPtoPEnd(
                    XyPtoPEndType.PassiveEnd,
                    pLocalEndPars, 
                    XyPtoPRequestHandler);

            //因用于对方配置远程地址，因此人使用remote key
            pLocalEndPars
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteIP, "192.168.168.129");
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteChatPort, "12921");
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteStreamPort, "12922");
            pLocalEndPars.Add(XyPtoPEnd.FolderparKey_hostName,
                System.Environment.MachineName);

            Dictionary<string, string> pRemoteEndPars
                = new Dictionary<string, string>();
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteIP, "192.168.168.129");
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteChatPort, "12919");
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteStreamPort, "12920");

            try
            {
                CommResult registResult = await xyPtoPEnd.Regist(
                    pLocalEndPars,
                    pRemoteEndPars
                    );

                if (registResult.errorCmdID)
                {
                    label1.Text = "Comm error";
                }
                else if (!registResult.cmdSucceed)
                {
                    label1.Text = "Regist error:" + registResult.resultDataDic[
                        CommResult.resultDataKey_ErrorInfo
                        ];
                }
                else
                {
                    label1.Text = registResult.resultDataDic[
                        CommResult.resultDataKey_ActiveEndInfo
                        ];
                }
                button1.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + ex.StackTrace);
                xyPtoPEnd.clean();
            }
        }

        private void XyPtoPRequestHandler(CommData commData, CommResult commResult)
        {
            showMsg("in request:" + commData.cmd.ToString());
            switch (commData.cmd)
            {
                case XyPtoPCmd.ActiveGetInitFolder:
                    string path = ".";

                    string[] subdirectoryEntries = Directory.GetDirectories(
                        path);
                    string folderStr = "";
                    foreach (string subdirectory in subdirectoryEntries)
                    {
                        if (folderStr != "")
                        {
                            folderStr += "|";
                        }
                        folderStr += Path.GetFileName(subdirectory);
                    }
                    commResult.resultDataDic.Add(
                        XyPtoPEnd.FolderparKey_folders, folderStr);

                    string[] fileEntries = Directory.GetFiles(
                        path);
                    string fileStr = "";
                    foreach (string fileName in fileEntries)
                    {
                        if (fileStr != "")
                        {
                            fileStr += "|";
                        }
                        fileStr += Path.GetFileName(fileName);
                    }
                    commResult.resultDataDic.Add(
                        XyPtoPEnd.FolderparKey_files, fileStr);
                    ;
                    showMsg("sent folder info:" + path);

                    break;
                default:
                    break;
            }
        }
        private void showMsg(string msg)
        {
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Action(() => { showMsg(msg); }));
            }
            else
            {
                label1.Text = msg;
            }
        }
    }
}
