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
            panel2 = new Panel();
            panel5 = new Panel();
            listView1 = new ListView();
            treeView1 = new TreeView();
            imageList1 = new ImageList(components);
            panel3 = new Panel();
            panelProgress = new Panel();
            progressBar1 = new ProgressBar();
            panel6 = new Panel();
            btnDownload = new Button();
            btnUpload = new Button();
            panel4 = new Panel();
            btnExit = new Button();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            labelProgress = new Label();
            panel2.SuspendLayout();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            panelProgress.SuspendLayout();
            panel6.SuspendLayout();
            panel4.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(panel5);
            panel2.Controls.Add(panel3);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(200, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(600, 450);
            panel2.TabIndex = 3;
            // 
            // panel5
            // 
            panel5.Controls.Add(listView1);
            panel5.Controls.Add(treeView1);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(600, 415);
            panel5.TabIndex = 2;
            // 
            // listView1
            // 
            listView1.Dock = DockStyle.Fill;
            listView1.Location = new Point(256, 0);
            listView1.Name = "listView1";
            listView1.Size = new Size(344, 415);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Left;
            treeView1.ImageIndex = 0;
            treeView1.ImageList = imageList1;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new Size(256, 415);
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
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 450);
            panel1.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Top;
            pictureBox1.Location = new Point(0, 17);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(200, 50);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 17);
            label1.TabIndex = 1;
            label1.Text = "label1";
            label1.TextAlign = ContentAlignment.TopCenter;
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
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "FrmMain";
            Text = "FrmMain";
            Load += FrmMain_Load;
            panel2.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panelProgress.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel2;
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
    }
}