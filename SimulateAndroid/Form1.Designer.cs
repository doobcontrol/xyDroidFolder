namespace SimulateAndroid
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            textBox1 = new TextBox();
            panel1 = new Panel();
            panelProgress = new Panel();
            labelProgress = new Label();
            progressBar1 = new ProgressBar();
            panel1.SuspendLayout();
            panelProgress.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 9);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Connect";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(0, 84);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(436, 261);
            textBox1.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(436, 35);
            panel1.TabIndex = 3;
            // 
            // panelProgress
            // 
            panelProgress.Controls.Add(labelProgress);
            panelProgress.Controls.Add(progressBar1);
            panelProgress.Dock = DockStyle.Top;
            panelProgress.Location = new Point(0, 35);
            panelProgress.Name = "panelProgress";
            panelProgress.Padding = new Padding(6);
            panelProgress.Size = new Size(436, 49);
            panelProgress.TabIndex = 5;
            // 
            // labelProgress
            // 
            labelProgress.Dock = DockStyle.Fill;
            labelProgress.Location = new Point(6, 29);
            labelProgress.Name = "labelProgress";
            labelProgress.Size = new Size(424, 14);
            labelProgress.TabIndex = 6;
            labelProgress.Text = "label1";
            labelProgress.TextAlign = ContentAlignment.TopCenter;
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Top;
            progressBar1.Location = new Point(6, 6);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(424, 23);
            progressBar1.TabIndex = 5;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(436, 345);
            Controls.Add(textBox1);
            Controls.Add(panelProgress);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panelProgress.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private Panel panel1;
        private Panel panelProgress;
        private Label labelProgress;
        private ProgressBar progressBar1;
    }
}
