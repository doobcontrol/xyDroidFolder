namespace xyDroidFolder
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            panelWork = new Panel();
            lbStatus = new Label();
            panel5 = new Panel();
            panelPopMessage = new Panel();
            panelTargetContent = new Panel();
            listView1 = new ListView();
            splitter1 = new Splitter();
            treeView1 = new TreeView();
            imageList1 = new ImageList(components);
            lbSelectedTargetPath = new Label();
            toolStrip1 = new ToolStrip();
            tsbRefreshCurrentNode = new ToolStripButton();
            tsbClipboardWatch = new ToolStripButton();
            panel3 = new Panel();
            panelProgress = new Panel();
            progressBar1 = new ProgressBar();
            labelProgress = new Label();
            panel6 = new Panel();
            btnDownload = new Button();
            btnUpload = new Button();
            panel4 = new Panel();
            btnExit = new Button();
            panel1 = new Panel();
            comboBox1 = new ComboBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            tsbOpenReceiveFolder = new ToolStripButton();
            panelWork.SuspendLayout();
            panel5.SuspendLayout();
            panelTargetContent.SuspendLayout();
            toolStrip1.SuspendLayout();
            panel3.SuspendLayout();
            panelProgress.SuspendLayout();
            panel6.SuspendLayout();
            panel4.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panelWork
            // 
            panelWork.Controls.Add(lbStatus);
            panelWork.Controls.Add(panel5);
            panelWork.Controls.Add(panel3);
            panelWork.Dock = DockStyle.Fill;
            panelWork.Location = new Point(200, 0);
            panelWork.Name = "panelWork";
            panelWork.Size = new Size(600, 450);
            panelWork.TabIndex = 3;
            // 
            // lbStatus
            // 
            lbStatus.Dock = DockStyle.Bottom;
            lbStatus.Location = new Point(0, 398);
            lbStatus.Name = "lbStatus";
            lbStatus.Size = new Size(600, 17);
            lbStatus.TabIndex = 6;
            lbStatus.Text = "label2";
            // 
            // panel5
            // 
            panel5.Controls.Add(panelPopMessage);
            panel5.Controls.Add(panelTargetContent);
            panel5.Controls.Add(lbSelectedTargetPath);
            panel5.Controls.Add(toolStrip1);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(600, 415);
            panel5.TabIndex = 2;
            // 
            // panelPopMessage
            // 
            panelPopMessage.Dock = DockStyle.Top;
            panelPopMessage.Location = new Point(0, 42);
            panelPopMessage.Name = "panelPopMessage";
            panelPopMessage.Size = new Size(600, 39);
            panelPopMessage.TabIndex = 6;
            // 
            // panelTargetContent
            // 
            panelTargetContent.Controls.Add(listView1);
            panelTargetContent.Controls.Add(splitter1);
            panelTargetContent.Controls.Add(treeView1);
            panelTargetContent.Location = new Point(60, 138);
            panelTargetContent.Name = "panelTargetContent";
            panelTargetContent.Size = new Size(447, 243);
            panelTargetContent.TabIndex = 3;
            // 
            // listView1
            // 
            listView1.Dock = DockStyle.Fill;
            listView1.Location = new Point(209, 0);
            listView1.Name = "listView1";
            listView1.Size = new Size(238, 243);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // splitter1
            // 
            splitter1.Location = new Point(199, 0);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(10, 243);
            splitter1.TabIndex = 3;
            splitter1.TabStop = false;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Left;
            treeView1.ImageIndex = 0;
            treeView1.ImageList = imageList1;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(199, 243);
            treeView1.TabIndex = 2;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "Folder-icon.png");
            imageList1.Images.SetKeyName(1, "Folder-Open-icon.png");
            imageList1.Images.SetKeyName(2, "Document-icon.png");
            imageList1.Images.SetKeyName(3, "android-icon.png");
            // 
            // lbSelectedTargetPath
            // 
            lbSelectedTargetPath.Dock = DockStyle.Top;
            lbSelectedTargetPath.Location = new Point(0, 25);
            lbSelectedTargetPath.Name = "lbSelectedTargetPath";
            lbSelectedTargetPath.Size = new Size(600, 17);
            lbSelectedTargetPath.TabIndex = 5;
            lbSelectedTargetPath.Text = "label2";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbRefreshCurrentNode, tsbClipboardWatch, tsbOpenReceiveFolder });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(600, 25);
            toolStrip1.TabIndex = 4;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsbRefreshCurrentNode
            // 
            tsbRefreshCurrentNode.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbRefreshCurrentNode.Image = Properties.Resources.Refresh;
            tsbRefreshCurrentNode.ImageTransparentColor = Color.Magenta;
            tsbRefreshCurrentNode.Name = "tsbRefreshCurrentNode";
            tsbRefreshCurrentNode.Size = new Size(23, 22);
            tsbRefreshCurrentNode.Text = "toolStripButton1";
            tsbRefreshCurrentNode.Click += tsbRefreshCurrentNode_Click;
            // 
            // tsbClipboardWatch
            // 
            tsbClipboardWatch.CheckOnClick = true;
            tsbClipboardWatch.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbClipboardWatch.Image = Properties.Resources.clipboard_icon;
            tsbClipboardWatch.ImageTransparentColor = Color.Magenta;
            tsbClipboardWatch.Name = "tsbClipboardWatch";
            tsbClipboardWatch.Size = new Size(23, 22);
            tsbClipboardWatch.Text = "toolStripButton1";
            tsbClipboardWatch.ToolTipText = "start/stop watch clipboard";
            tsbClipboardWatch.CheckedChanged += tsbClipboardWatch_CheckedChanged;
            // 
            // panel3
            // 
            panel3.Controls.Add(panelProgress);
            panel3.Controls.Add(panel6);
            panel3.Controls.Add(panel4);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(0, 415);
            panel3.Name = "panel3";
            panel3.Size = new Size(600, 35);
            panel3.TabIndex = 0;
            // 
            // panelProgress
            // 
            panelProgress.Controls.Add(progressBar1);
            panelProgress.Controls.Add(labelProgress);
            panelProgress.Dock = DockStyle.Fill;
            panelProgress.Location = new Point(169, 0);
            panelProgress.Name = "panelProgress";
            panelProgress.Padding = new Padding(3);
            panelProgress.Size = new Size(341, 35);
            panelProgress.TabIndex = 4;
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Fill;
            progressBar1.Location = new Point(3, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(335, 12);
            progressBar1.TabIndex = 0;
            // 
            // labelProgress
            // 
            labelProgress.Dock = DockStyle.Bottom;
            labelProgress.Location = new Point(3, 15);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new Size(335, 17);
            labelProgress.TabIndex = 1;
            labelProgress.Text = "label2";
            labelProgress.TextAlign = ContentAlignment.TopCenter;
            // 
            // panel6
            // 
            panel6.Controls.Add(btnDownload);
            panel6.Controls.Add(btnUpload);
            panel6.Dock = DockStyle.Left;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(169, 35);
            panel6.TabIndex = 3;
            // 
            // btnDownload
            // 
            btnDownload.Location = new Point(6, 3);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(75, 23);
            btnDownload.TabIndex = 1;
            btnDownload.Text = "btnDownload";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_ClickAsync;
            // 
            // btnUpload
            // 
            btnUpload.Location = new Point(87, 3);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(75, 23);
            btnUpload.TabIndex = 2;
            btnUpload.Text = "btnUpload";
            btnUpload.UseVisualStyleBackColor = true;
            btnUpload.Click += btnUpload_Click;
            // 
            // panel4
            // 
            panel4.Controls.Add(btnExit);
            panel4.Dock = DockStyle.Right;
            panel4.Location = new Point(510, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(90, 35);
            panel4.TabIndex = 0;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(3, 3);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(75, 23);
            btnExit.TabIndex = 0;
            btnExit.Text = "btnExit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(3, 0, 0, 0);
            panel1.Size = new Size(200, 450);
            panel1.TabIndex = 2;
            // 
            // comboBox1
            // 
            comboBox1.Dock = DockStyle.Top;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(3, 67);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(197, 25);
            comboBox1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Top;
            pictureBox1.Location = new Point(3, 17);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(197, 50);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(197, 17);
            label1.TabIndex = 1;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // tsbOpenReceiveFolder
            // 
            tsbOpenReceiveFolder.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbOpenReceiveFolder.Image = Properties.Resources.folder_star_icon;
            tsbOpenReceiveFolder.ImageTransparentColor = Color.Magenta;
            tsbOpenReceiveFolder.Name = "tsbOpenReceiveFolder";
            tsbOpenReceiveFolder.Size = new Size(23, 22);
            tsbOpenReceiveFolder.Text = "toolStripButton1";
            tsbOpenReceiveFolder.Click += tsbOpenReceiveFolder_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelWork);
            Controls.Add(panel1);
            Name = "FrmMain";
            Text = "FrmMain";
            Load += FrmMain_Load;
            panelWork.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panelTargetContent.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel3.ResumeLayout(false);
            panelProgress.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelWork;
        private Panel panel3;
        private Panel panel4;
        private Button btnExit;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label1;
        private Panel panel5;
        private TreeView treeView1;
        private ListView listView1;
        private ImageList imageList1;
        private Button btnUpload;
        private Button btnDownload;
        private Panel panelProgress;
        private ProgressBar progressBar1;
        private Panel panel6;
        private Label labelProgress;
        private ComboBox comboBox1;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbRefreshCurrentNode;
        private Panel panelTargetContent;
        private Splitter splitter1;
        private Label lbSelectedTargetPath;
        private ToolStripButton tsbClipboardWatch;
        private Label lbStatus;
        private Panel panelPopMessage;
        private ToolStripButton tsbOpenReceiveFolder;
    }
}