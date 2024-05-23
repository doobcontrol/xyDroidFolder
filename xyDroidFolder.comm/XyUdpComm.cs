﻿using System;
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

        UdpClient udpChatServer;
        UdpClient udpStreamServer; //for file transfer
        IPEndPoint targetChatPoint;
        IPEndPoint targetStreamPoint;

        private event EventHandler<XyCommEventArgs> XyCommEventHandler;

        public XyUdpComm(Dictionary<string, string> workpars) {

            IPEndPoint localChatEndPoint = new IPEndPoint(
                IPAddress.Parse(workpars[workparKey_localIP]),
                int.Parse(workpars[workparKey_localChatPort]));

            udpChatServer = new UdpClient(localChatEndPoint);

            IPEndPoint localStreamEndPoint = new IPEndPoint(
                IPAddress.Parse(workpars[workparKey_localIP]),
                int.Parse(workpars[workparKey_localStreamPort]));
            udpStreamServer = new UdpClient(localStreamEndPoint);

            int workBufferSize = 1024 * 32;
            udpStreamServer.Client.ReceiveBufferSize = workBufferSize;
            udpStreamServer.Client.SendBufferSize = workBufferSize;

            //配置socket参数，避免ICMP包导致的异常
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            udpChatServer.Client.IOControl(
                (int)SIO_UDP_CONNRESET,
                new byte[] { Convert.ToByte(false) },
                null
                );

            //udpChatServer.Connect(targetEndPoint);
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
            udpStreamServer.Send(
                sendBytes,
                sendLength,
                targetStreamPoint
                );
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

                        XyCommEventHandler(
                            this,
                            new XyCommEventArgs(
                                receivedBytes,
                                XyCommEventMsgType.Stream)
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
    }
}
