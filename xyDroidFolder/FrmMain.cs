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

namespace xyDroidFolder
{
    public partial class FrmMain : Form
    {
        bool isDebug = false;

        int qrSize = 200;
        int port = 12919;

        public FrmMain()
        {
            InitializeComponent();

            this.Text = Properties.Resources.AppName;

            panel1.Width = qrSize;
            pictureBox1.Height = qrSize;

            button1.Text = Properties.Resources.App_Exit;
            label1.Text = Properties.Resources.ScanTitle;

            try
            {
                pictureBox1.Image = getQRImage();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private Bitmap getQRImage()
        {
            var writer = new ZXing.Windows.Compatibility.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions { Height = qrSize, Width = qrSize }
            };

            Bitmap retImg = writer.Write(getLocalIp()[0] + ":" + port);
            return retImg;
        }

        private List<string> getLocalIp()
        {
            string host = Dns.GetHostName();

            // Getting ip address using host name 
            IPHostEntry ip = Dns.GetHostEntry(host);

            List<string> ipList = new List<string>();
            foreach (var item in ip.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipList.Add(item.ToString());
                }
            }
            return ipList;
        }
    }
}
