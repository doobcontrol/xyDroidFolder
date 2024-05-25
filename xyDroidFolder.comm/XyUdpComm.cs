using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace xyDroidFolder.comm
{
    public class XyUdpComm : IXyComm
    {
        static public string workparKey_localIP = "localIP";
        static public string workparKey_localChatPort = "localChatPort";
        static public string workparKey_localStreamPort = "localStreamPort";
        static public string workparKey_remoteIP = "remoteIP";
        static public string workparKey_remoteChatPort = "remoteChatPort";
        static public string workparKey_remoteStreamPort = "remoteStreamPort";

        IPAddress localIPAddress;
        UdpClient udpChatServer;
        UdpClient udpStreamServer; //for file transfer
        IPEndPoint targetChatPoint;
        IPEndPoint targetStreamPoint;

        private event EventHandler<XyCommEventArgs> XyCommEventHandler;

        public XyUdpComm(Dictionary<string, string> workpars) {
            localIPAddress = IPAddress.Parse(workpars[workparKey_localIP]);

            IPEndPoint localChatEndPoint = new IPEndPoint(
                IPAddress.Parse(workpars[workparKey_localIP]),
                int.Parse(workpars[workparKey_localChatPort]));

            udpChatServer = new UdpClient(localChatEndPoint);

            IPEndPoint localStreamEndPoint = new IPEndPoint(
                IPAddress.Parse(workpars[workparKey_localIP]),
                int.Parse(workpars[workparKey_localStreamPort]));
            udpStreamServer = new UdpClient(localStreamEndPoint);

            int workBufferSize = 1024 * 1024;
            udpStreamServer.Client.ReceiveBufferSize = workBufferSize;
            //udpStreamServer.Client.SendBufferSize = workBufferSize;

            //配置socket参数，避免ICMP包导致的异常
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            udpChatServer.Client.IOControl(
                (int)SIO_UDP_CONNRESET,
                new byte[] { Convert.ToByte(false) },
                null
                );
        }

        #region IXyComm

        public void send(string sendData)
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);
            udpChatServer.Send(
                sendBytes, 
                sendBytes.Length,
                targetChatPoint
                );
        }

        public void send(Dictionary<string, string> sendDic)
        {
            string sendString = "";
            foreach (string pName in sendDic.Keys)
            {
                if (sendString != "")
                {
                    sendString += ",";
                }
                sendString += pName + "=" + sendDic[pName];
            }
            send(sendString);
        }

        public void sendStream(byte[] sendBytes, int sendLength)
        {
            UdpClient uc = new UdpClient(new IPEndPoint(
                localIPAddress,
                0));
            uc.Connect(targetStreamPoint);
            uc.Send(
                sendBytes,
                sendLength);

            uc.Close();
        }
        public async Task<bool> sendNumberedStream(
            byte[] sendBytes,
            int sendLength,
            Int64 sendNumber)
        {
            byte[] idByte = BitConverter.GetBytes(sendNumber);
            idByte.CopyTo(sendBytes, sendLength);
            byte[] lengthByte = BitConverter.GetBytes(sendLength);
            lengthByte.CopyTo(sendBytes, sendLength + 8);

            UdpClient uc = new UdpClient(new IPEndPoint(
                localIPAddress,
                0));
            uc.Client.ReceiveTimeout = 10000;
            uc.Connect(targetStreamPoint);
            uc.Send(
                sendBytes,
                sendLength + 8 + 8);

            byte[]? receivedBytes = null;
            await Task.Run(
            () =>
            {
                IPEndPoint tempPoint = new IPEndPoint(
                    targetStreamPoint.Address,
                    targetStreamPoint.Port
                    );
                try
                {
                    receivedBytes = uc.Receive(ref tempPoint);
                }
                catch (Exception e)
                {
                    d("uc.Receive: " + e.Message + " : " + e.StackTrace);
                }
            });

            uc.Close();

            if(receivedBytes == null)
            {
                return false;
            }
            else
            {
                Int64 retrunNumber = BitConverter.ToInt64(receivedBytes);

                int receivedLength = receivedBytes.Length;
                byte[] lengthByteArr = new byte[8];
                lengthByteArr[0] = receivedBytes[receivedLength - 8];
                lengthByteArr[1] = receivedBytes[receivedLength - 7];
                lengthByteArr[2] = receivedBytes[receivedLength - 6];
                lengthByteArr[3] = receivedBytes[receivedLength - 5];
                lengthByteArr[4] = receivedBytes[receivedLength - 4];
                lengthByteArr[5] = receivedBytes[receivedLength - 3];
                lengthByteArr[6] = receivedBytes[receivedLength - 2];
                lengthByteArr[7] = receivedBytes[receivedLength - 1];

                Int64 receivedSendLength = BitConverter.ToInt64(lengthByteArr);

                if (retrunNumber == sendNumber 
                    && receivedSendLength == sendLength)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void start(Dictionary<string, string> pars)
        {
            throw new NotImplementedException();
        }

        bool stopListen = true;
        private int sleepTime = 50;
        public void startChatListen()
        {
            stopListen = false;
            _ = Task.Run(
            () =>
            {
                try
                {
                    while (!stopListen)
                    {
                        while (!stopListen && udpChatServer.Available == 0)
                        {
                            Thread.Sleep(sleepTime);
                        }
                        if (stopListen)
                        {
                            break;
                        }

                        IPEndPoint tempPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receivedBytes 
                            = udpChatServer.Receive(ref tempPoint); //??

                        XyCommEventHandler(
                            this, 
                            new XyCommEventArgs(
                                receivedBytes,
                                XyCommEventMsgType.Chat)
                            );

                        Thread.Sleep(sleepTime);
                    }
                }
                catch (ThreadAbortException te)
                {
                    //dReceivedEventHandler(
                    //    PDMnetMsg.invalidPkgInfo
                    //    + "=接收线程错误：" + te.Message + "-" + te.StackTrace);
                    ////EventHandler(EventType.mErr, "已停止连接", null, null, null);
                }
                catch (Exception e)
                {
                    //dReceivedEventHandler(
                    //    PDMnetMsg.invalidPkgInfo
                    //    + "=接收线程错误：" + e.Message + "-" + e.StackTrace.Replace(",", "，"));
                    ////EventHandler(EventType.oErr, e.Message, null, null, e);
                }
            }
            );
        }
        public void startStreamListen()
        {
            stopListen = false;
            _ = Task.Run(
            () =>
            {
                try
                {
                    while (!stopListen)
                    {
                        while (!stopListen && udpStreamServer.Available == 0)
                        {
                            Thread.Sleep(sleepTime);
                        }
                        if (stopListen)
                        {
                            break;
                        }

                        IPEndPoint tempPoint = new IPEndPoint(IPAddress.Any, 0);
                        byte[] receivedBytes
                            = udpStreamServer.Receive(ref tempPoint); //??

                        //return pkg id
                        byte[] returnBytes = new byte[8 + 8];
                        int receivedLength = receivedBytes.Length;
                        returnBytes[0] = receivedBytes[receivedLength - 16];
                        returnBytes[1] = receivedBytes[receivedLength - 15];
                        returnBytes[2] = receivedBytes[receivedLength - 14];
                        returnBytes[3] = receivedBytes[receivedLength - 13];
                        returnBytes[4] = receivedBytes[receivedLength - 12];
                        returnBytes[5] = receivedBytes[receivedLength - 11];
                        returnBytes[6] = receivedBytes[receivedLength - 10];
                        returnBytes[7] = receivedBytes[receivedLength - 9];
                        returnBytes[8] = receivedBytes[receivedLength - 8];
                        returnBytes[9] = receivedBytes[receivedLength - 7];
                        returnBytes[10] = receivedBytes[receivedLength - 6];
                        returnBytes[11] = receivedBytes[receivedLength - 5];
                        returnBytes[12] = receivedBytes[receivedLength - 4];
                        returnBytes[13] = receivedBytes[receivedLength - 3];
                        returnBytes[14] = receivedBytes[receivedLength - 2];
                        returnBytes[15] = receivedBytes[receivedLength - 1];
                        udpStreamServer.Send(
                            returnBytes,
                            16,
                            tempPoint
                            );

                        byte[] lengthByteArr = new byte[8];
                        lengthByteArr[0] = receivedBytes[receivedLength - 8];
                        lengthByteArr[1] = receivedBytes[receivedLength - 7];
                        lengthByteArr[2] = receivedBytes[receivedLength - 6];
                        lengthByteArr[3] = receivedBytes[receivedLength - 5];
                        lengthByteArr[4] = receivedBytes[receivedLength - 4];
                        lengthByteArr[5] = receivedBytes[receivedLength - 3];
                        lengthByteArr[6] = receivedBytes[receivedLength - 2];
                        lengthByteArr[7] = receivedBytes[receivedLength - 1];

                        Int64 lengthByte = BitConverter.ToInt64(lengthByteArr);

                        if (lengthByte == receivedLength - 16)
                        {
                            XyCommEventHandler(
                                this,
                                new XyCommEventArgs(
                                    receivedBytes,
                                    XyCommEventMsgType.Stream)
                                );
                        }
                    }
                }
                catch (ThreadAbortException te)
                {
                    d("ThreadAbortException te: "
                        + te.Message + "" + te.StackTrace);
                }
                catch (Exception e)
                {
                    d("Exception e: "
                        + e.Message + "" + e.StackTrace);
                }
            }
            );
        }

        public void setTargetEndPoint(Dictionary<string, string> pEndPars)
        {
            targetChatPoint = new IPEndPoint(
                IPAddress.Parse(pEndPars[workparKey_remoteIP]),
                int.Parse(pEndPars[workparKey_remoteChatPort]));
            targetStreamPoint = new IPEndPoint(
                IPAddress.Parse(pEndPars[workparKey_remoteIP]),
                int.Parse(pEndPars[workparKey_remoteStreamPort]));
        }

        public void setCommEventHandler(EventHandler<XyCommEventArgs> xyCommEventHandler)
        {
            XyCommEventHandler += xyCommEventHandler;
        }

        public void clean()
        {
            stopListen = true;
            if (udpChatServer != null)
            {
                udpChatServer.Close();
                udpChatServer = null;
            }
            if (udpStreamServer != null)
            {
                udpStreamServer.Close();
                udpStreamServer = null;
            }
        }

        #endregion

        //debug
        static public void d(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
}
