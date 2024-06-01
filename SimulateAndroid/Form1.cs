using System.IO;
using xyDroidFolder.comm;

namespace SimulateAndroid
{
    public partial class Form1 : Form
    {
        DroidFolderComm droidFolderComm;

        string localIP = "192.168.168.129";
        int localPort = 12921;
        //stream receiver listen port for other side to connect
        string streamReceiverPar = "12922";

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
                remoteIP, remotePort, DroidFolderRequestHandler,
                FileEventHandler
                );

            try
            {
                CommResult registResult = await droidFolderComm.Register(
                    localIP,
                    localPort,
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
                case DroidFolderCmd.GetInitFolder:
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
                        CmdPar.folders.ToString(), folderStr);

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
                        CmdPar.files.ToString(), fileStr);
                    ;
                    showMsg("sent folder info: " + path);

                    break;
                case DroidFolderCmd.GetFolder:
                    string requestfolder =
                        commData.cmdParDic[CmdPar.requestPath.ToString()];
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
                        CmdPar.folders.ToString(), folderStr);

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
                        CmdPar.files.ToString(), fileStr);
                    ;
                    showMsg("sent folder info: " + reqPath);

                    break;
                case DroidFolderCmd.GetFile:
                    string requestfile = Path.Combine(
                            path,
                            commData.cmdParDic[CmdPar.targetFile.ToString()]);

                    commData.cmdParDic[CmdPar.targetFile.ToString()]
                        = requestfile;

                    commResult.resultDataDic.Add(
                        CmdPar.fileLength.ToString(),
                        new FileInfo(requestfile).Length.ToString());

                    showMsg("start send file: " + requestfile);

                    break;
                case DroidFolderCmd.SendFile:
                    string sendfile = Path.Combine(
                            path,
                            commData.cmdParDic[CmdPar.targetFile.ToString()]);

                    commData.cmdParDic[CmdPar.targetFile.ToString()]
                        = sendfile;

                    commResult.resultDataDic.Add(
                        CmdPar.streamReceiverPar.ToString(),
                        streamReceiverPar);

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
        
        private void FileEventHandler(object? sender, XyFileIOEventArgs e)
        {
            if (panelProgress.InvokeRequired)
            {
                panelProgress.Invoke(new Action(() => {
                    FileEventHandler(sender, e);
                }));
            }
            else
            {
                switch (e.Type)
                {
                    case FileIOEventType.start:
                        progressBar1.Maximum = (int)e.Length;
                        progressBar1.Value = 0;
                        panelProgress.Visible = true;
                        break;
                    case FileIOEventType.end:
                        panelProgress.Visible = false;
                        progressBar1.Maximum = 0;
                        progressBar1.Value = 0;
                        break;
                    case FileIOEventType.progress:
                        if (panelProgress.Visible)
                        {
                            progressBar1.Value = (int)e.Progress;
                        }
                        break;
                }

                if (panelProgress.Visible)
                {
                    labelProgress.Text = e.FileSendReceive.ToString() + ": " +
                        "(" + ((float)progressBar1.Value / (float)progressBar1.Maximum)
                        .ToString(("#0.00%")) + ") "
                        + progressBar1.Value.ToString("##,#")
                        + "/" + progressBar1.Maximum.ToString("##,#");
                }
            }
        }
    }
}
