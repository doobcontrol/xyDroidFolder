﻿using System;
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

namespace xyDroidFolder
{
    public partial class FrmMain : Form
    {
        bool isDebug = true;

        int qrSize = 200;
        int chatPort = 12919;
        int streamPort = 12920;

        string localIP;

        XyPtoPEnd xyPtoPEnd;

        public FrmMain()
        {
            InitializeComponent();

            R.setCulture(""); //zh-CN

            this.Text = R.AppName;

            treeView1.Visible = false;
            listView1.Visible = false;

            panel1.Width = qrSize;
            pictureBox1.Height = qrSize;

            btnExit.Text = R.App_Exit;
            label1.Text = R.ScanTitle;

            localIP = getLocalIp()[0];

            try
            {
                pictureBox1.Image = getQRImage(localIP, chatPort, streamPort);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if (isDebug)
            {
                runEmulator();
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> pEndPars
                = new Dictionary<string, string>();
            pEndPars.Add(XyUdpComm.workparKey_localIP, localIP);
            pEndPars.Add(XyUdpComm.workparKey_localChatPort, chatPort.ToString());
            pEndPars.Add(XyUdpComm.workparKey_localStreamPort, streamPort.ToString());

            xyPtoPEnd = new XyPtoPEnd(
                XyPtoPEndType.ActiveEnd,
                pEndPars,
                XyPtoPRequestHandler
                );

        }

        private Bitmap getQRImage(
            string ip,
            int chatPort,
            int streamPort
            )
        {
            var writer = new ZXing.Windows.Compatibility.BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions { Height = qrSize, Width = qrSize }
            };

            Bitmap retImg = writer.Write(
                ip + ":" + chatPort + ":" + streamPort
                );
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

        private void XyPtoPRequestHandler(CommData commData, CommResult commResult)
        {
            switch (commData.cmd)
            {
                case XyPtoPCmd.PassiveRegist:
                    initFoldTree(treeView1);
                    break;
                default:
                    break;
            }
        }

        private void initFoldTree(System.Windows.Forms.TreeView treeView)
        {
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new Action(() => { initFoldTree(treeView1); }));
            }
            else
            {
                treeView1.Visible = true;
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
    }
}
