﻿using System.ComponentModel;
using System.Net.Sockets;
using System.Net;
using ZXing.Common;
using ZXing;
using xyDroidFolder.comm;
using System.Diagnostics;
using TreeView = System.Windows.Forms.TreeView;
using ComboBox = System.Windows.Forms.ComboBox;
using xyDroidFolder.clipboard;
using xySoft.comm;
using Button = System.Windows.Forms.Button;
using System.Collections;
using System.Globalization;
using xySoft.log;

namespace xyDroidFolder
{
    public partial class FrmMain : Form
    {
        bool isDebug = false; //true / false

        string receivedPath = ".\\temp";

        int qrSize = 200;
        int Port = 12919;
        //stream receiver listen port for other side to connect
        string streamReceiverPar = "12920";

        DroidFolderComm? droidFolderComm;

        public FrmMain()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.xyfolder;

            lbVersion.Text = "V" + Application.ProductVersion.Split("+")[0];
            lbStatus.Text = "";
            lbStatus.Visible = false;
            lbStatus.BorderStyle = BorderStyle.FixedSingle;

            panelPopMessage.Visible = false;
            panelPopMessage.BorderStyle = BorderStyle.FixedSingle;

            tsbClipboardWatch.Checked = false;

            panelWork.Visible = false;
            labelProgress.Text = "0/0";

            panel1.Width = qrSize;
            pictureBox1.Height = qrSize;

            lbSelectedTargetPath.Text = null;

            btnDownload.Enabled = false;
            btnUpload.Enabled = false;

            panelTargetContent.Dock = DockStyle.Fill;

            configLanguage();

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
            if (droidFolderComm != null)
            {
                droidFolderComm.clean();
            }
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
            try
            {
                switch (commData.cmd)
                {
                    case DroidFolderCmd.Register:
                        initFoldTree(
                            treeView1,
                            commData.cmdParDic[CmdPar.hostName.ToString()]);
                        recoverUi();
                        break;
                    case DroidFolderCmd.SendText:
                        //do not check if need handle for now
                        string receivedText =
                            commData.cmdParDic[CmdPar.text.ToString()];
                        putToClipboard(receivedText);
                        PopMessageByinvoke(R.Text_received_msg
                            + receivedText);
                        break;
                    case DroidFolderCmd.SendFile:
                        string path = ".\\temp";
                        string sendfile = Path.Combine(
                                path,
                                commData.cmdParDic[CmdPar.targetFile.ToString()]);

                        commData.cmdParDic[CmdPar.targetFile.ToString()]
                            = sendfile;

                        commResult.resultDataDic.Add(
                            CmdPar.streamReceiverPar.ToString(),
                            streamReceiverPar);

                        //if file too tiny, message show by showStatusMessage disappear too soon
                        PopMessageByinvoke(R.Received_File + Path.GetFullPath(sendfile));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                XyLog.log(e);
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

                btnDownload.Enabled = false;
                btnUpload.Enabled = false;
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

                lbSelectedTargetPath.Text = null;
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

            lbSelectedTargetPath.Text = tn.FullPath;

            if (NodeNeedGet(tn))
            {
                await refreshNodeAsync(tn);
            }
            else
            {
                showFiles(tn);
            }

            if (tn.Tag != null)
            {
                btnUpload.Enabled = true;
            }
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
            List<string> files = tn.Tag as List<string>;

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
            _ = downloadSelectedFild();
        }
        private async Task downloadSelectedFild()
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }

            panel4.Enabled = false;
            panel5.Enabled = false;
            panel6.Enabled = false;
            ControlBox = false;

            TreeNode tn = listView1.Tag as TreeNode;
            string path = tn.FullPath;
            string file = listView1.SelectedItems[0].Text;

            string requestfile = Path.Combine(path, file).Replace(
                treeView1.Tag.ToString() + "\\", "");

            if (!Directory.Exists(receivedPath))
            {
                Directory.CreateDirectory(receivedPath);
            }
            string receivedFile = Path.Combine(receivedPath, file);

            try
            {
                showStatusMessage(R.Request_download + requestfile);
                await droidFolderComm.GetFile(
                        receivedFile, requestfile, streamReceiverPar);

                PopMessage(R.Download_succeed_msg + Path.GetFullPath(receivedFile));

                //open file
                using Process process =
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(receivedFile)
                        {
                            UseShellExecute = true
                        }
                    };
                process.Start();
            }
            catch (DroidFolderCommException dfce)
            {
                PopMessage(R.Error_msg + dfce.Message);
                hideStatusMessage();
            }
            catch (Exception ex)
            {
                PopMessage(R.Error_msg + ex.Message);
                hideStatusMessage();
            }
            finally
            {
                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                ControlBox = true;
            }
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

                try
                {
                    showStatusMessage(R.Request_upload + localFile);
                    await droidFolderComm.SendFile(
                            localFile, remoteFile);
                }
                catch (DroidFolderCommException dfce)
                {
                    PopMessage(R.Error_msg + dfce.Message);
                    hideStatusMessage();
                    return;
                }
                finally
                {
                    panel4.Enabled = true;
                    panel5.Enabled = true;
                    panel6.Enabled = true;
                    ControlBox = true;
                }

                PopMessage(R.Upload_succeed_msg + localFile);

                refreshNodeAsync(tn);
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
                        hideStatusMessageByinvoke();
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
                    string progressName = "";
                    switch (e.FileSendReceive)
                    {
                        case XyCommFileSendReceive.Receive:
                            progressName = R.Receive_file_progress;
                            break;
                        case XyCommFileSendReceive.Send:
                            progressName = R.Send_file_progress;
                            break;
                    }
                    labelProgress.Text = progressName +
                        "(" + ((float)progressBar1.Value / (float)progressBar1.Maximum)
                        .ToString(("#0.00%")) + ") "
                        + progressBar1.Value.ToString("##,#")
                        + "/" + progressBar1.Maximum.ToString("##,#");
                }
            }
        }

        private void tsbRefreshCurrentNode_Click(object sender, EventArgs e)
        {
            if (listView1.Tag != null)
            {
                TreeNode tn = listView1.Tag as TreeNode;
                refreshNodeAsync(tn);
            }
        }
        private async Task refreshNodeAsync(TreeNode tn)
        {
            showStatusMessage(R.Getting_folder_content_msg);
            TreeView tv = treeView1;
            CommResult commResult = null;
            string path = "\\";
            if (tn.Level == 0)
            {
                try
                {
                    commResult =
                            await droidFolderComm.GetInitFolder();
                }
                catch (DroidFolderCommException dfce)
                {
                    PopMessage(R.Error_msg + dfce.Message);
                    hideStatusMessage();
                    return;
                }

                tn.Text = tv.Tag.ToString();
                lbSelectedTargetPath.Text = tn.FullPath;
            }
            else
            {

                path = tn.FullPath.Replace(
                    tv.Tag.ToString() + "\\", "");
                try
                {
                    commResult =
                        await droidFolderComm.GetFolder(path);
                }
                catch (DroidFolderCommException dfce)
                {
                    PopMessage(R.Error_msg + dfce.Message);
                    hideStatusMessage();
                    return;
                }
            }


            List<string> folders = commResult.resultDataDic[
                CmdPar.folders.ToString()
                ].Split("|").ToList<string>();
            for (int i = 0; i < folders.Count; i++)
            {
                folders[i] = folders[i];
            }

            List<string> files = commResult.resultDataDic[
                CmdPar.files.ToString()
                ].Split("|").ToList<string>();
            for (int i = 0; i < files.Count; i++)
            {
                files[i] = files[i];
            }

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

            if (folders.Count > 0)
            {
                tn.Expand();
            }

            //show files
            tn.Tag = files;
            showFiles(tn);

            PopMessage(R.Get_folder_succeed_msg + ": " + path);
            hideStatusMessage();
        }

        private void tsbClipboardWatch_CheckedChanged(object sender, EventArgs e)
        {
            if (tsbClipboardWatch.Checked)
            {
                tsbClipboardWatch.ToolTipText = R.tsbClipboardWatch_tooltip_inWatch;
            }
            else
            {
                tsbClipboardWatch.ToolTipText = R.tsbClipboardWatch_tooltip_notWatch;
            }

            changeMonitorStatus(tsbClipboardWatch.Checked);
        }

        private void showStatusMessage(string msg)
        {
            lbStatus.Text = msg;
            lbStatus.Visible = true;
        }
        private void hideStatusMessage()
        {
            lbStatus.Text = "";
            lbStatus.Visible = false;
        }
        private void hideStatusMessageByinvoke()
        {
            if (lbStatus.InvokeRequired)
            {
                lbStatus.Invoke(() => hideStatusMessage());
            }
            else
            {
                hideStatusMessage();
            }
        }
        private async void PopMessage(string msg)
        {
            XyLog.log(msg);
            hideStatusMessageByinvoke();

            Panel msgPanel = new Panel();
            msgPanel.Dock = DockStyle.Top;
            msgPanel.Height = 23;

            Button closeBtn = new Button();
            closeBtn.Dock = DockStyle.Left;
            closeBtn.Size = new Size(23, 22);
            closeBtn.Image = Properties.Resources.Close;

            CancelMsg cancelMsg = () =>
            {
                panelPopMessage.Height = panelPopMessage.Height - msgPanel.Height;
                panelPopMessage.Controls.Remove(msgPanel);
                if (panelPopMessage.Controls.Count == 0)
                {
                    panelPopMessage.Visible = false;
                }
            };
            closeBtn.Click += (object sender, EventArgs e) =>
            {
                cancelMsg();
            };

            Label msgLabel = new Label();
            msgLabel.AutoSize = false;
            msgLabel.Dock = DockStyle.Fill;
            msgLabel.TextAlign = ContentAlignment.MiddleLeft;
            msgLabel.Text = msg;

            msgPanel.Controls.Add(msgLabel);
            msgPanel.Controls.Add(closeBtn);

            panelPopMessage.Controls.Add(msgPanel);
            panelPopMessage.Height = msgPanel.Height * panelPopMessage.Controls.Count + 1;
            panelPopMessage.Visible = true;

            await Task.Run(() => Thread.Sleep(5000));
            cancelMsg();
        }
        delegate void CancelMsg();
        private void PopMessageByinvoke(string msg)
        {
            if (panelPopMessage.InvokeRequired)
            {
                panelPopMessage.Invoke(() => PopMessage(msg));
            }
            else
            {
                PopMessage(msg);
            }
        }

        private void tsbOpenReceiveFolder_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(receivedPath))
            {
                Directory.CreateDirectory(receivedPath);
            }
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            System.Diagnostics.Process prc = new System.Diagnostics.Process();
            prc.StartInfo.FileName = windir + @"\explorer.exe";
            prc.StartInfo.Arguments = receivedPath;// outputFolder;
            prc.Start();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            _ = downloadSelectedFild();
        }

        #region config Language
        private void configLanguage()
        {
            BindingList<DictionaryEntry> LanguageList = new BindingList<DictionaryEntry>();
            LanguageList.Add(new DictionaryEntry("en-US", "English (United States)"));
            LanguageList.Add(new DictionaryEntry("zh-CN", "中文 (中国)"));
            cmbLanguage.DataSource = LanguageList;
            cmbLanguage.DisplayMember = "Value";
            cmbLanguage.ValueMember = "Key";
            cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguage.SelectedIndexChanged += configLanguage_SelectedIndexChanged;

            string? langConfig = XyConfig.getOnePar("langConfig");
            if (langConfig == null)
            {
                langConfig = CultureInfo.CurrentUICulture.Name;
            }

            if (langConfig == cmbLanguage.SelectedValue.ToString())
            {
                changeLanguage(langConfig);
            }
            else
            {
                cmbLanguage.SelectedValue = langConfig;
            }
        }
        private void configLanguage_SelectedIndexChanged(object? sender, EventArgs e)
        {
            ComboBox cb = cmbLanguage;
            if (cb.SelectedValue != null)
            {
                string langConfig = cb.SelectedValue.ToString();
                XyConfig.setOnePar("langConfig", langConfig);

                changeLanguage(langConfig);
            }
        }
        private void changeLanguage(string langConfig)
        {
            R.setCulture(langConfig);

            this.Text = R.AppName;

            tsbRefreshCurrentNode.ToolTipText = R.tsbRefreshCurrentNode_tooltip;
            tsbOpenReceiveFolder.ToolTipText = R.tsbOpenReceiveFolder_tooltip;
            tsbClipboardWatch.ToolTipText = R.tsbClipboardWatch_tooltip_notWatch;

            btnExit.Text = R.App_Exit;
            btnDownload.Text = R.FileBtn_Download;
            btnUpload.Text = R.FileBtn_Upload;
            label1.Text = R.ScanTitle;
        }
        #endregion

        #region watch clipboard

        bool InClipboardMonitor = false;

        ClipboardMonitor cm;
        protected override void WndProc(ref Message m)
        {
            if (InClipboardMonitor && cm != null)
            {
                if (!cm.WndProc(ref m))
                {
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void changeMonitorStatus(bool inClipboardMonitor)
        {
            if (cm == null)
            {
                cm = new ClipboardMonitor();
                cm.ClipboardMsgHandler += ClipboardText_get;
            }

            InClipboardMonitor = inClipboardMonitor;
            cm.changeMonitorStatus(InClipboardMonitor, Handle);
        }
        private void ClipboardText_get(object sender, EventArgs e)
        {
            if (!InClipboardMonitor)
            {
                return;
            }

            string ClipboardString = ((ClipboardMonitor)sender).ClipboardString;
            if (ClipboardString != null && InClipboardMonitor)
            {
                droidFolderComm.SendText(ClipboardString);
                PopMessageByinvoke(R.Sent_clipboard_text_msg + ClipboardString);
            }

        }
        private void putToClipboard(string putString)
        {
            bool tempBool = InClipboardMonitor;
            InClipboardMonitor = false;

            //deal with "Current thread must be set to single thread apartment (STA)" error
            Thread thread = new Thread(new ThreadStart(() =>
            {
                Clipboard.SetText(putString);
            }));
            thread.SetApartmentState(ApartmentState.STA); //important
            thread.Start();
            thread.Join(); //Wait for the thread to end

            InClipboardMonitor = tempBool;
        }
        #endregion
    }
}
