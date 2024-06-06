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
        event EventHandler<XyCommFileEventArgs> _xyCommFileEventHandler;

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
            uc.ExclusiveAddressUse = true;
            uc.Send(sendBytes, sendLength, targetPoint);

            //read Response
            IPEndPoint tempPoint = new IPEndPoint(
                targetPoint.Address,
                targetPoint.Port);
            byte[] receivedBytes = await Task.Run(() => {
                    return uc.Receive(ref tempPoint);
                });

            uc.Close();
            uc = null;

            return receivedBytes;
        }

        public async Task<byte[]> sendBytesForResponseAsync(byte[] sendBytes)
        {
            return await sendBytesForResponseAsync(sendBytes, sendBytes.Length);
        }

        public async Task<bool> sendNumberedStream(
            byte[] sendBytes,
            int sendLength,
            Int64 sendNumber,
            int sendPort)
        {
            byte[] idByte = BitConverter.GetBytes(sendNumber);
            idByte.CopyTo(sendBytes, sendLength);
            byte[] lengthByte = BitConverter.GetBytes(sendLength);
            lengthByte.CopyTo(sendBytes, sendLength + 8);

            UdpClient uc = new UdpClient();
            uc.ExclusiveAddressUse = true;
            uc.Client.ReceiveTimeout = 5000;
            uc.Connect(new IPEndPoint(targetPoint.Address, sendPort));
            uc.Send(
                sendBytes,
                sendLength + 8 + 8);

            byte[]? receivedBytes = null;
            await Task.Run(
            () =>
            {
                IPEndPoint tempPoint = new IPEndPoint(
                    targetPoint.Address,
                    sendPort
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

            if (receivedBytes == null)
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

        #region IXyComm

        public async Task<string> sendForResponseAsync(string sendData)
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);

            byte[] returnBytes = await sendBytesForResponseAsync(sendBytes);

            return Encoding.UTF8.GetString(
                        returnBytes, 0, returnBytes.Length);
        }

        bool stopListen = true;
        UdpClient udpServer;
        public void startListen()
        {
            udpServer = new UdpClient();
            udpServer.ExclusiveAddressUse = true;
            udpServer.Client.Bind(localPoint);

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

                        byte[] receivedBytes = udpServer.Receive(
                            ref tempPoint); //??

                        string receivedString = Encoding.UTF8.GetString(
                            receivedBytes, 0, receivedBytes.Length
                            );

                        string resultString = _xyCommRequestHandler(receivedString);
                        if (resultString != null)
                        {
                            udpServer.Send(
                                Encoding.UTF8.GetBytes(
                                    resultString
                                ),
                                tempPoint
                                );
                        }
                    }
                }
                catch (ThreadAbortException te)
                {
                }
                catch (Exception e)
                {
                }
                finally {
                    udpServer = null;
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

        public Task prepareStreamReceiver(
            string file,
            string fileLength,
            string streamReceiverPar, 
            EventHandler<XyCommFileEventArgs> xyCommFileEventHandler
            )
        {
            _xyCommFileEventHandler = xyCommFileEventHandler;

            int receivePort = int.Parse(streamReceiverPar);
            UdpClient udpFileServer = new UdpClient();
            udpFileServer.ExclusiveAddressUse = true;
            udpFileServer.Client.Bind(
                new IPEndPoint(localPoint.Address, receivePort)
                );

            //write file task
            Dictionary<Int64, byte[]> receivedByteArrs = 
                new Dictionary<long, byte[]>();
            Int64 WaitedPkgID = 0;

            long receviedLength = 0;
            long totleSendFileLength = long.Parse(fileLength);

            FileStream receiveFileStream = new FileStream(
                file,
                FileMode.Create,
                FileAccess.Write);

            Task writeTask = Task.Run(() => {
                while (true)
                {
                    try
                    {
                        if (receivedByteArrs.Count > 0
                        && totleSendFileLength > receviedLength)
                        {
                            if (receivedByteArrs.ContainsKey(WaitedPkgID))
                            {
                                byte[] tByte = receivedByteArrs[WaitedPkgID];
                                receiveFileStream.Write(
                                    tByte,
                                    0,
                                    tByte.Length - 8 - 8);
                                receviedLength += tByte.Length - 8 - 8;

                                lock (receivedByteArrs)
                                {
                                    receivedByteArrs.Remove(WaitedPkgID);
                                }
                                WaitedPkgID++;

                                _xyCommFileEventHandler(
                                    this,
                                    new XyCommFileEventArgs(
                                        XyCommFileSendReceive.Receive,
                                        totleSendFileLength,
                                        receviedLength
                                        )
                                    );
                            }
                        }

                        if (totleSendFileLength == receviedLength)
                        {
                            udpFileServer.Close();
                            d("writeTask done");
                            break;
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

                if (receiveFileStream != null)
                {
                    receiveFileStream.Close();
                }
            });

            //receive task
            d("start listenning: " + localPoint.Address + ":" + receivePort);

            _ = Task.Run(
            () =>
            {
                try
                {
                    while (true)
                    {
                        IPEndPoint tempPoint
                            = new IPEndPoint(IPAddress.Any, 0);

                        byte[] receivedBytes = udpFileServer.Receive(
                            ref tempPoint); //??

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
                        udpFileServer.Send(
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
                            byte[] pkgID = new byte[8];
                            pkgID[0] = receivedBytes[receivedLength - 16];
                            pkgID[1] = receivedBytes[receivedLength - 15];
                            pkgID[2] = receivedBytes[receivedLength - 14];
                            pkgID[3] = receivedBytes[receivedLength - 13];
                            pkgID[4] = receivedBytes[receivedLength - 12];
                            pkgID[5] = receivedBytes[receivedLength - 11];
                            pkgID[6] = receivedBytes[receivedLength - 10];
                            pkgID[7] = receivedBytes[receivedLength - 9];

                            Int64 pkgIDNumber = BitConverter.ToInt64(pkgID);

                            lock (receivedByteArrs)
                            {
                                if (pkgIDNumber >= WaitedPkgID
                                    && !receivedByteArrs.ContainsKey(pkgIDNumber))
                                {
                                    receivedByteArrs.Add(pkgIDNumber, receivedBytes);
                                }
                            }
                        }

                        if (totleSendFileLength == receviedLength)
                        {
                            d("receiveTask done");
                            break;
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

            return writeTask;
        }

        public Task sendFile(
            string file,
            string fileLength,
            string streamReceiverPar,
            EventHandler<XyCommFileEventArgs> xyCommFileEventHandler
            )
        {
            _xyCommFileEventHandler = xyCommFileEventHandler;
            int sendPort = int.Parse(streamReceiverPar);
            int maxBufferTaskCount = 50;
            int maxSendTaskCount = 30;

            List<fileSendTask> sendTaskDataList
                = new List<fileSendTask>();
            Dictionary<Int64, fileSendTask> inSendTaskDataDic
                = new Dictionary<long, fileSendTask>();

            Dictionary<Int64, Task> sendTasksDic
                = new Dictionary<Int64, Task>();

            Task sendFileTask = Task.Run(
            async Task () =>
            {
                try
                {
                    int sendLength = 1024 * 32;
                    int bufferSize = sendLength + 8 + 8; //+sendID+sendLength
                    byte[] filechunk;
                    int numBytes;
                    long sentBytes = 0;

                    long receivedSentBytes = 0;
                    long totleSendFileLength = long.Parse(fileLength);
                    FileStream fileStream = new FileStream(file, FileMode.Open);

                    bool isReadFinish = false;
                    Int64 sendNumber = 0;
                    while (true)
                    {
                        if (!isReadFinish)
                        {
                            if (sendTaskDataList.Count <= maxBufferTaskCount)
                            {
                                filechunk = new byte[bufferSize];
                                numBytes =
                                    fileStream.Read(filechunk, 0, sendLength);
                                if (numBytes > 0)
                                {
                                    sendTaskDataList.Add(
                                            new fileSendTask(
                                                sendNumber,
                                                filechunk,
                                                numBytes)
                                            );
                                    sendNumber++;
                                }
                                else
                                {
                                    fileStream.Close();
                                    isReadFinish = true;
                                }
                            }
                        }

                        if (sendTaskDataList.Count > 0
                            && sendTasksDic.Count <= maxSendTaskCount)
                        {
                            fileSendTask fst = sendTaskDataList.First();
                            lock (inSendTaskDataDic)
                            {
                                inSendTaskDataDic.Add(fst.SendNumber, fst);
                            }
                            sendTaskDataList.Remove(fst);
                            Task tempTask = Task.Run(async () => {
                                bool sendSucceed = false;
                                while (!sendSucceed)
                                {
                                    try
                                    {
                                        sendSucceed =
                                        await sendNumberedStream(
                                            fst.SendBytes,
                                            fst.SendLength,
                                            fst.SendNumber,
                                            sendPort
                                            );
                                    }
                                    catch (Exception e)
                                    {
                                        d("myIXyComm.sendNumberedStream: "
                                            + e.Message + "" + e.StackTrace);
                                    }
                                }
                                receivedSentBytes += fst.SendLength;
                                _xyCommFileEventHandler(
                                    this,
                                    new XyCommFileEventArgs(
                                        XyCommFileSendReceive.Send,
                                        totleSendFileLength,
                                        receivedSentBytes
                                        )
                                    );
                                lock (inSendTaskDataDic)
                                {
                                    inSendTaskDataDic.Remove(fst.SendNumber);
                                }

                                lock (sendTasksDic)
                                {
                                    sendTasksDic.Remove(fst.SendNumber);
                                }
                            });
                            lock (sendTasksDic)
                            {
                                sendTasksDic.Add(fst.SendNumber, tempTask);
                            }
                        }

                        if (isReadFinish && sendTaskDataList.Count == 0)
                        {
                            await Task.Run(() => {
                                while (sendTasksDic.Count > 0) ;
                            });

                            _xyCommFileEventHandler(
                                this,
                                new XyCommFileEventArgs(
                                    XyCommFileSendReceive.Send,
                                    totleSendFileLength,
                                    totleSendFileLength // why not equal receivedSentBytes？
                                    )
                                );

                            d("all file pkg sent!");
                            break;
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

            return sendFileTask;
        }

        #endregion

        //debug
        static public void d(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        void IXyComm.stopListen()
        {
            stopListen = true;
            if (udpServer != null)
            {
                udpServer.Client.Close();
            }
        }
    }
    public enum XyUdpCommTargetSetPar
    {
        ip,
        port
    }
    public struct fileSendTask
    {
        public fileSendTask(
            Int64 sendNumber,
            byte[] sendBytes,
            int sendLength)
        {
            SendNumber = sendNumber;
            SendBytes = sendBytes;
            SendLength = sendLength;
        }
        public Int64 SendNumber;
        public byte[] SendBytes;
        public int SendLength;
    }
}
