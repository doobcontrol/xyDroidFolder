using System.IO;
using xyDroidFolder.comm;

namespace SimulateAndroid
{
    public partial class Form1 : Form
    {
        DroidFolderComm droidFolderComm;

        string localIP = "192.168.168.129";
        int localPort = 12921;
        string remoteIP = "192.168.168.129"; //"192.168.168.129"  "192.168.168.1"
        int remotePort = 12919;

        public Form1()
        {
            InitializeComponent();
            Text = "SimulateAndroid";
            panelProgress.Visible= false;
            labelProgress.Text = "0/0";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            showMsg("start regist бнбн");

            droidFolderComm = new DroidFolderComm(
                localIP, localPort,
                remoteIP, remotePort, DroidFolderRequestHandler
                );

            try
            {
                CommResult registResult = await droidFolderComm.Register(
                    remoteIP,
                    remotePort,
                    System.Environment.MachineName
                    );

                if (registResult.errorCmdID)
                {
                    showMsg("Comm error");
                }
                else if (!registResult.cmdSucceed)
                {
                    showMsg("Regist error: " + registResult.resultDataDic[
                        CmdPar.errorMsg.ToString()
                        ]);
                }
                else
                {
                    showMsg(registResult.resultDataDic[
                        CmdPar.returnMsg.ToString()
                        ]);
                }
                button1.Enabled = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + ex.StackTrace);
                //xyPtoPEnd.clean();
            }
        }

        string path = "C:\\";
        private void DroidFolderRequestHandler(CommData commData, CommResult commResult)
        {
            showMsg("in request: " + commData.cmd.ToString());
            string[] subdirectoryEntries;
            string folderStr = "";
            string[] fileEntries;
            string fileStr = "";
            switch (commData.cmd)
            {

                case DroidFolderCmd.Register:
                    break;

                //case XyPtoPCmd.ActiveGetInitFolder:

                //    subdirectoryEntries = Directory.GetDirectories(
                //        path);
                //    foreach (string subdirectory in subdirectoryEntries)
                //    {
                //        if (folderStr != "")
                //        {
                //            folderStr += "|";
                //        }
                //        folderStr += Path.GetFileName(subdirectory);
                //    }
                //    commResult.resultDataDic.Add(
                //        XyPtoPEnd.FolderparKey_folders, folderStr);

                //    fileEntries = Directory.GetFiles(
                //        path);
                //    foreach (string fileName in fileEntries)
                //    {
                //        if (fileStr != "")
                //        {
                //            fileStr += "|";
                //        }
                //        fileStr += Path.GetFileName(fileName);
                //    }
                //    commResult.resultDataDic.Add(
                //        XyPtoPEnd.FolderparKey_files, fileStr);
                //    ;
                //    showMsg("sent folder info: " + path);

                //    break;
                //case XyPtoPCmd.ActiveGetFolder:
                //    string requestfolder = 
                //        commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfolder];
                //    string reqPath = Path.Combine(path, requestfolder);
                //    subdirectoryEntries = Directory.GetDirectories(
                //        reqPath);
                //    foreach (string subdirectory in subdirectoryEntries)
                //    {
                //        if (folderStr != "")
                //        {
                //            folderStr += "|";
                //        }
                //        folderStr += Path.GetFileName(subdirectory);
                //    }
                //    commResult.resultDataDic.Add(
                //        XyPtoPEnd.FolderparKey_folders, folderStr);

                //    fileEntries = Directory.GetFiles(
                //        reqPath);
                //    foreach (string fileName in fileEntries)
                //    {
                //        if (fileStr != "")
                //        {
                //            fileStr += "|";
                //        }
                //        fileStr += Path.GetFileName(fileName);
                //    }
                //    commResult.resultDataDic.Add(
                //        XyPtoPEnd.FolderparKey_files, fileStr);
                //    ;
                //    showMsg("sent folder info: " + reqPath);

                //    break;
                //case XyPtoPCmd.ActiveGetFile:
                //    string requestfile = Path.Combine(
                //            path,
                //            commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfile]);

                //    commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfile]
                //        = requestfile;

                //    showMsg("start send file: " + requestfile);

                //    break;
                //case XyPtoPCmd.ActiveSendFile:
                //    string sendfile = Path.Combine(
                //            path,
                //            commData.cmdParDic[XyPtoPEnd.FolderparKey_sendfile]);

                //    commData.cmdParDic[XyPtoPEnd.FolderparKey_sendfile]
                //        = sendfile;

                //    showMsg("ready to receive file: " + sendfile);

                //    break;

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
