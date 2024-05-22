using xyDroidFolder.comm;

namespace SimulateAndroid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = "SimulateAndroid";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "start regist ……";

            Dictionary<string, string> pLocalEndPars 
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_localIP, "192.168.168.129");
            pLocalEndPars.Add(XyUdpComm.workparKey_localChatPort, "12921");
            pLocalEndPars.Add(XyUdpComm.workparKey_localStreamPort, "12922");

            XyPtoPEnd xyPtoPEnd = new XyPtoPEnd(
                    XyPtoPEndType.PassiveEnd,
                    pLocalEndPars, 
                    XyPtoPRequestHandler);

            //因用于对方配置远程地址，因此人使用remote key
            pLocalEndPars
                = new Dictionary<string, string>();
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteIP, "192.168.168.129");
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteChatPort, "12921");
            pLocalEndPars.Add(XyUdpComm.workparKey_remoteStreamPort, "12922");

            Dictionary<string, string> pRemoteEndPars
                = new Dictionary<string, string>();
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteIP, "192.168.168.129");
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteChatPort, "12919");
            pRemoteEndPars.Add(XyUdpComm.workparKey_remoteStreamPort, "12920");

            try
            {
                CommResult registResult = await xyPtoPEnd.Regist(
                    pLocalEndPars,
                    pRemoteEndPars
                    );

                if (registResult.errorCmdID)
                {
                    label1.Text = "Comm error";
                }
                else if (!registResult.cmdSucceed)
                {
                    label1.Text = "Regist error:" + registResult.resultDataDic[
                        CommResult.resultDataKey_ErrorInfo
                        ];
                }
                else
                {
                    label1.Text = registResult.resultDataDic[
                        CommResult.resultDataKey_ActiveEndInfo
                        ];
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + ex.StackTrace);
            }

            xyPtoPEnd.clean();

        }

        private void XyPtoPRequestHandler(CommData commData, CommResult commResult)
        {

        }
    }
}
