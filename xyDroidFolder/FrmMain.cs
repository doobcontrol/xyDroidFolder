using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing.Common;
using ZXing;
using xyDroidFolder.comm;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TreeView = System.Windows.Forms.TreeView;
using ComboBox = System.Windows.Forms.ComboBox;
using System.Reflection;

namespace xyDroidFolder
{
    public partial class FrmMain : Form
    {
        bool isDebug = true; //true / false

        int qrSize = 200;
        int Port = 12919;
        //stream receiver listen port for other side to connect
        string streamReceiverPar = "12920";

        DroidFolderComm? droidFolderComm;

        public FrmMain()
        {
            InitializeComponent();

            R.setCulture(""); //zh-CN

            this.Icon = Properties.Resources.xyfolder;
            this.Text = R.AppName;

            tsbRefreshCurrentNode.ToolTipText = R.tsbRefreshCurrentNode_tooltip;

            panelWork.Visible = false;
            labelProgress.Text = "0/0";

            panel1.Width = qrSize;
            pictureBox1.Height = qrSize;

            btnExit.Text = R.App_Exit;
            btnDownload.Text = R.FileBtn_Download;
            btnUpload.Text = R.FileBtn_Upload;
            label1.Text = R.ScanTitle;

            btnDownload.Enabled = false;
            btnUpload.Enabled = false;

            if (isDebug)
            {
                runEmulator();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            getLocalIp();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndexChanged += localIP_SelectedIndexChanged;
            comboBox1.SelectedIndex = 0;
        }
        private void localIP_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ComboBox cb = comboBox1;
            if (cb.SelectedIndex != -1)
            {
                setLocalIpEndPoint(
                    cb.Items[cb.SelectedIndex].ToString()
                    );
            }
        }

        private void setLocalIpEndPoint(string localIP)
        {
            droidFolderComm = new DroidFolderComm(
                localIP, Port,
                null, 0, DroidFolderRequestHandler,
                FileEventHandler
                );

            try
            {
                pictureBox1.Image = getQRImage(localIP, Port);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private Bitmap getQRImage(
            string ip,
            int port
            )
        {
            var writer = new ZXing.Windows.Compatibility.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions { Height = qrSize, Width = qrSize }
            };

            Bitmap retImg = writer.Write(
                ip + ":" + port
                );
            return retImg;
        }

        private void getLocalIp()
        {
            string host = Dns.GetHostName();

            // Getting ip address using host name 
            IPHostEntry ip = Dns.GetHostEntry(host);

            foreach (var item in ip.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    comboBox1.Items.Add(item.ToString());
                }
            }
        }

        private void DroidFolderRequestHandler(CommData commData, CommResult commResult)
        {
            switch (commData.cmd)
            {
                case DroidFolderCmd.Register:
                    initFoldTree(
                        treeView1,
                        commData.cmdParDic[CmdPar.hostName.ToString()]);
                    recoverUi();
                    break;
                default:
                    break;
            }
        }
        //from test fail(all item disabbled)
        private void recoverUi()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    recoverUi();
                }));
            }
            else
            {
                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                ControlBox = true;

                panelProgress.Visible = false;
            }
        }

        private void initFoldTree(TreeView treeView, string hostName)
        {
            if (panelWork.InvokeRequired)
            {
                panelWork.Invoke(new Action(() =>
                {
                    initFoldTree(treeView, hostName);
                }));
            }
            else
            {
                treeView.Tag = hostName;
                treeView.BeginUpdate();
                treeView.Nodes.Clear();
                TreeNode tn = treeView.Nodes.Add(
                    hostName + "(" + R.InitFolderNode + ")");
                tn.ImageIndex = 3;
                tn.SelectedImageIndex = 3;
                treeView.EndUpdate();

                panelWork.Visible = true;
                treeView.Visible = true;

                panelProgress.Visible = false;
                listView1.Tag = null;
                listView1.Items.Clear();
                listView1.Visible = false;
            }
        }

        static public void runEmulator()
        {
            Process[] pname = Process.GetProcessesByName("SimulateAndroid");
            if (pname.Length == 0)
            {
                string Emulator =
                    "..\\..\\..\\..\\SimulateAndroid\\bin\\Debug\\net8.0-windows\\SimulateAndroid.exe";
                Process.Start(Emulator);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tn = e.Node;
            TreeView tv = treeView1;


            if (NodeNeedGet(tn))
            {
                await refreshNodeAsync(tn);
            }
            else
            {
                showFiles(tn);
            }
            btnUpload.Enabled = true;
        }
        private bool NodeNeedGet(TreeNode tn)
        {
            if (tn.Tag == null)
            {
                return true;
            }
            else if (tn.Tag is string[])
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void showFiles(TreeNode tn)
        {
            listView1.Clear();
            btnDownload.Enabled = false;
            listView1.Tag = tn;
            string[] files = tn.Tag as string[];

            // Set the view to show details.
            listView1.View = View.List;

            listView1.Columns.Add("name", -2, HorizontalAlignment.Left);

            foreach (string file in files)
            {
                listView1.Items.Add(file);
            }
            listView1.Visible = true;
        }

        private async void btnDownload_ClickAsync(object sender, EventArgs e)
        {
            panel4.Enabled = false;
            panel5.Enabled = false;
            panel6.Enabled = false;
            ControlBox = false;

            TreeNode tn = listView1.Tag as TreeNode;
            string path = tn.FullPath;
            string file = listView1.SelectedItems[0].Text;

            string requestfile = Path.Combine(path, file).Replace(
                treeView1.Tag.ToString() + "\\", "");

            string receivedPath = ".\\temp";
            if (!Directory.Exists(receivedPath))
            {
                Directory.CreateDirectory(receivedPath);
            }
            string receivedFile = Path.Combine(receivedPath, file); ;

            await droidFolderComm.GetFile(
                    receivedFile, requestfile, streamReceiverPar);

            panel4.Enabled = true;
            panel5.Enabled = true;
            panel6.Enabled = true;
            ControlBox = true;
        }

        private async void btnUpload_Click(object sender, EventArgs e)
        {
            FileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                panel4.Enabled = false;
                panel5.Enabled = false;
                panel6.Enabled = false;
                ControlBox = false;

                string localFile = dlg.FileName;

                TreeNode tn = listView1.Tag as TreeNode;
                string path = tn.FullPath;

                string remoteFile = Path.Combine(
                    path, Path.GetFileName(dlg.FileName)
                    )
                    .Replace(treeView1.Tag.ToString() + "\\", "");

                await droidFolderComm.SendFile(
                        localFile, remoteFile);

                refreshNodeAsync(tn);

                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                ControlBox = true;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                btnDownload.Enabled = true;
            }
            else
            {
                btnDownload.Enabled = false;
            }
        }

        private void FileEventHandler(object? sender, XyFileIOEventArgs e)
        {
            if (panelProgress.InvokeRequired)
            {
                panelProgress.Invoke(new Action(() =>
                {
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

        private void tsbRefreshCurrentNode_Click(object sender, EventArgs e)
        {
            if(listView1.Tag!=null)
            {
                TreeNode tn = listView1.Tag as TreeNode; 
                refreshNodeAsync(tn);
            }
        }
        private async Task refreshNodeAsync(TreeNode tn)
        {
            TreeView tv = treeView1;
            CommResult commResult;
            if (tn.Level == 0)
            {
                commResult =
                        await droidFolderComm.GetInitFolder();
                tn.Text = tv.Tag.ToString();
            }
            else
            {
                string path = tn.FullPath.Replace(
                    tv.Tag.ToString() + "\\", "");
                commResult =
                    await droidFolderComm.GetFolder(path);
            }


            string[] folders = commResult.resultDataDic[
                CmdPar.folders.ToString()
                ].Split("|");
            string[] files = commResult.resultDataDic[
                CmdPar.files.ToString()
                ].Split("|");

            tv.BeginUpdate();
            TreeNode TempTn;

            tn.Nodes.Clear();
            foreach (string folder in folders)
            {
                if (folder != "")
                {
                    TempTn = tn.Nodes.Add(folder);
                }
            }

            tv.EndUpdate();

            if (folders.Length > 0)
            {
                tn.Expand();
            }

            //show files
            tn.Tag = files;
            showFiles(tn);
        }
    }
}
