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
            label1.Text = "start regist ����";

            Dictionary<string, string> pLocalEndPars 
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_localIP, "192.168.168.129");
            pLocalEndPars.Add(XyUdpComm.workparKey_localChatPort, "12921");
            pLocalEndPars.Add(XyUdpComm.workparKey_localStreamPort, "12922");

            xyPtoPEnd = new XyPtoPEnd(
                    XyPtoPEndType.PassiveEnd,
                    pLocalEndPars, 
                    XyPtoPRequestHandler);

            //�����ڶԷ�����Զ�̵�ַ�������ʹ��remote key
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

        string path = "C:\\DepGitHub\\xyDroidFolder";
        private void XyPtoPRequestHandler(CommData commData, CommResult commResult)
        {
            showMsg("in request:" + commData.cmd.ToString());
            string[] subdirectoryEntries;
            string folderStr = "";
            string[] fileEntries;
            string fileStr = "";
            switch (commData.cmd)
            {
                case XyPtoPCmd.ActiveGetInitFolder:

                    subdirectoryEntries = Directory.GetDirectories(
                        path);
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

                    fileEntries = Directory.GetFiles(
                        path);
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
                case XyPtoPCmd.ActiveGetFolder:
                    string requestfolder = 
                        commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfolder];
                    string reqPath = Path.Combine(path, requestfolder);
                    subdirectoryEntries = Directory.GetDirectories(
                        reqPath);
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

                    fileEntries = Directory.GetFiles(
                        reqPath);
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
                    showMsg("sent folder info:" + reqPath);

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
