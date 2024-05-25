using System.IO;
using xyDroidFolder.comm;

namespace SimulateAndroid
{
    public partial class Form1 : Form
    {
        XyPtoPEnd xyPtoPEnd;

        string localIP = "192.168.168.129";
        string localChatPort = "12921";
        string localStreamPort = "12922";
        string remoteIP = "192.168.168.1";
        string remoteChatPort = "12919";
        string remoteStreamPort = "12920";

        public Form1()
        {
            InitializeComponent();
            Text = "SimulateAndroid";
            panelProgress.Visible= false;
            labelProgress.Text = "0/0";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            showMsg("start regist ……");

            Dictionary<string, string> pLocalEndPars 
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_localIP, localIP);
            pLocalEndPars.Add(XyUdpComm.workparKey_localChatPort, localChatPort);
            pLocalEndPars.Add(XyUdpComm.workparKey_localStreamPort, localStreamPort);

            xyPtoPEnd = new XyPtoPEnd(
                    XyPtoPEndType.PassiveEnd,
                    pLocalEndPars, 
                    XyPtoPRequestHandler,
                    FileEventHandler);

            //因用于对方配置远程地址，因此仍使用remote key
            pLocalEndPars
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteIP, localIP);
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteChatPort, localChatPort);
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteStreamPort, localStreamPort);
            pLocalEndPars.Add(XyPtoPEnd.FolderparKey_hostName,
                System.Environment.MachineName);

            Dictionary<string, string> pRemoteEndPars
                = new Dictionary<string, string>();
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteIP, remoteIP);
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteChatPort, remoteChatPort);
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteStreamPort, remoteStreamPort);

            try
            {
                CommResult registResult = await xyPtoPEnd.Regist(
                    pLocalEndPars,
                    pRemoteEndPars
                    );

                if (registResult.errorCmdID)
                {
                    showMsg("Comm error");
                }
                else if (!registResult.cmdSucceed)
                {
                    showMsg("Regist error: " + registResult.resultDataDic[
                        CommResult.resultDataKey_ErrorInfo
                        ]);
                }
                else
                {
                    showMsg(registResult.resultDataDic[
                        CommResult.resultDataKey_ActiveEndInfo
                        ]);
                }
                button1.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + ex.StackTrace);
                xyPtoPEnd.clean();
            }
        }

        string path = "C:\\";
        private void XyPtoPRequestHandler(CommData commData, CommResult commResult)
        {
            showMsg("in request: " + commData.cmd.ToString());
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
                    showMsg("sent folder info: " + path);

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
                    showMsg("sent folder info: " + reqPath);

                    break;
                case XyPtoPCmd.ActiveGetFile:
                    string requestfile = Path.Combine(
                            path,
                            commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfile]);

                    commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfile]
                        = requestfile;

                    showMsg("start send file: " + requestfile);

                    break;
                case XyPtoPCmd.ActiveSendFile:
                    string sendfile = Path.Combine(
                            path,
                            commData.cmdParDic[XyPtoPEnd.FolderparKey_sendfile]);

                    commData.cmdParDic[XyPtoPEnd.FolderparKey_sendfile]
                        = sendfile;

                    showMsg("ready to receive file: " + sendfile);

                    break;

                default:
                    break;
            }
        }
        private void showMsg(string msg)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke(new Action(() => { showMsg(msg); }));
            }
            else
            {
                textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - "
                    + msg + Environment.NewLine);
            }
        }
        
        private void FileEventHandler(object? sender, XyCommFileEventArgs e)
        {
            if (panelProgress.InvokeRequired)
            {
                panelProgress.Invoke(new Action(() => {
                    FileEventHandler(sender, e);
                }));
            }
            else
            {
                if (e.Progress == 0)
                {
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = (int)e.Length;
                    progressBar1.Value = 0;
                    panelProgress.Visible = true;
                }
                else if (e.Progress == e.Length)
                {
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = 0;
                    progressBar1.Value = 0;
                    panelProgress.Visible = false;
                }
                else if (progressBar1.Maximum > (int)e.Progress)
                {
                    progressBar1.Value = (int)e.Progress;
                }
                labelProgress.Text = e.FileSendReceive.ToString() + ": " +
                    "(" + ((float)progressBar1.Value / (float)progressBar1.Maximum)
                    .ToString(("#0.00%")) + ") "
                    + progressBar1.Value.ToString("##,#") 
                    + "/" + progressBar1.Maximum.ToString("##,#");
            }
        }
    }
}
