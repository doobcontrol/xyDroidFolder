using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Unicode;

namespace xySoft.comm
{
    public class XyUdpComm : IXyComm
    {
        IPEndPoint localPoint;
        IPEndPoint targetPoint;
        XyCommRequestHandler _xyCommRequestHandler;

        public XyUdpComm(
            string localIp, int localPort,
            string targetIp, int targetPort,
            XyCommRequestHandler xyCommRequestHandler
            ) {
            _xyCommRequestHandler = xyCommRequestHandler;
            localPoint = new IPEndPoint(IPAddress.Parse(localIp), localPort);
            if (targetIp != null)
            {
                targetPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
            }
        }

        public async Task<byte[]> sendBytesForResponseAsync(byte[] sendBytes, int sendLength)
        {
            UdpClient uc = new UdpClient();
            uc.Send(sendBytes, sendLength, targetPoint);

            //read Response
            IPEndPoint tempPoint = new IPEndPoint(
                targetPoint.Address,
                targetPoint.Port);
            byte[] receivedBytes
                = await Task.Run(() => {
                    return uc.Receive(ref tempPoint);
                });

            uc.Close();

            return receivedBytes;
        }

        public async Task<byte[]> sendBytesForResponseAsync(byte[] sendBytes)
        {
            return await sendBytesForResponseAsync(sendBytes, sendBytes.Length);
        }


        #region IXyComm

        public async Task<string> sendForResponseAsync(string sendData)
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);

            byte[] returnBytes = await sendBytesForResponseAsync(sendBytes);

            return Encoding.UTF8.GetString(
                        returnBytes, 0, returnBytes.Length);
        }

        bool stopListen = true;
        public void startListen()
        {
            UdpClient udpServer = new UdpClient(localPoint);

            stopListen = false;
            _ = Task.Run(
            () =>
            {
                try
                {
                    while (!stopListen)
                    {
                        IPEndPoint tempPoint 
                            = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receivedBytes
                            = udpServer.Receive(ref tempPoint); //??
                        
                        string receivedString =
                            Encoding.UTF8.GetString(
                                receivedBytes, 0, receivedBytes.Length);

                        byte[] sendBytes = Encoding.UTF8.GetBytes(
                            _xyCommRequestHandler(receivedString)
                            );

                        udpServer.Send(
                            sendBytes,
                            tempPoint
                            );
                    }
                }
                catch (ThreadAbortException te)
                {
                }
                catch (Exception e)
                {
                }
            }
            );
        }

        public void set(Dictionary<string, string> setDic)
        {
            targetPoint = new IPEndPoint(
                IPAddress.Parse(setDic[XyUdpCommTargetSetPar.ip.ToString()]),
                int.Parse(setDic[XyUdpCommTargetSetPar.port.ToString()]));
        }

        #endregion
    }
    public enum XyUdpCommTargetSetPar
    {
        ip,
        port
    }
}
