using xyDroidFolder.comm;

namespace SimulateAndroid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "start regist бнбн";

            Dictionary<string, string> pEndPars 
                = new Dictionary<string, string>();
            pEndPars.Add(XyUdpComm.workparKey_localIP, "192.168.168.129");
            pEndPars.Add(XyUdpComm.workparKey_localChatPort, "12921");
            pEndPars.Add(XyUdpComm.workparKey_localStreamPort, "12922");

            XyPtoPEnd xyPtoPEnd 
                = new XyPtoPEnd(XyPtoPEndType.PassiveEnd, null);

            pEndPars
                = new Dictionary<string, string>();
            pEndPars.Add(XyUdpComm.workparKey_localIP, "192.168.168.129");
            pEndPars.Add(XyUdpComm.workparKey_localChatPort, "12919");
            pEndPars.Add(XyUdpComm.workparKey_localStreamPort, "12920");
            CommResult registResult = await xyPtoPEnd.Regist();

            label1.Text = "Succeed regist";
        }
    }
}
