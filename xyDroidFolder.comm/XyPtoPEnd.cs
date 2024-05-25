using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace xyDroidFolder.comm
{
    public class XyPtoPEnd
    {
        private IXyComm myIXyComm;
        private XyPtoPEndType myXyPtoPEndType;
        private XyPtoPRequestHandler _xyPtoPRequestHandler;
        private event EventHandler<XyCommFileEventArgs> 
            _xyPtoPFileEventHandler;
        public XyPtoPEnd(
            XyPtoPEndType endType, 
            Dictionary<string, string> pEndPars,
            XyPtoPRequestHandler xyPtoPRequestHandler,
            EventHandler<XyCommFileEventArgs> xyPtoPFileEventHandler)
        {
            myXyPtoPEndType = endType;
            _xyPtoPRequestHandler = xyPtoPRequestHandler;
            _xyPtoPFileEventHandler = xyPtoPFileEventHandler;

            myIXyComm = new XyUdpComm(pEndPars); //配置实现类？
            myIXyComm.setCommEventHandler(myIXyCommEventRaise);

            myIXyComm.startChatListen();
            myIXyComm.startStreamListen();
        }

        private void myIXyCommEventRaise(object? sender, XyCommEventArgs e)
        {
            if(e.MsgType == XyCommEventMsgType.Chat)
            {
                string receivedString =
                    Encoding.UTF8.GetString(
                        e.ReceivedBytes, 0, e.ReceivedBytes.Length);

                string[] pars = receivedString.Split(',');
                Dictionary<string, string> parsDic = new Dictionary<string, string>();
                foreach (string par in pars)
                {
                    string[] parArr = par.Split('=');
                    parsDic.Add(parArr[0], parArr[1]);
                }

                if (parsDic.ContainsKey(CommData.commDicKey_cmd))
                {
                    //cmd send by other end. need reply
                    CommData commData = CommData.fromReceivedData(parsDic);

                    CommResult commResult = new CommResult(commData);
                    switch (commData.cmd)
                    {
                        //cmd from PassiveEnd
                        case XyPtoPCmd.PassiveRegist:

                            myIXyComm.setTargetEndPoint(commData.cmdParDic);

                            _xyPtoPRequestHandler(commData, commResult);

                            commResult.resultDataDic.Add(
                                CommResult.resultDataKey_ActiveEndInfo,
                                "Regist ok");
                            commResult.cmdSucceed = true;
                            break;

                        //cmd from ActiveEnd
                        case XyPtoPCmd.ActiveGetInitFolder:
                            _xyPtoPRequestHandler(commData, commResult);
                            commResult.cmdSucceed = true;
                            break;
                        case XyPtoPCmd.ActiveGetFolder:
                            _xyPtoPRequestHandler(commData, commResult);
                            commResult.cmdSucceed = true;
                            break;
                        case XyPtoPCmd.ActiveGetFile:
                            _xyPtoPRequestHandler(commData, commResult);

                            string sendFileName
                                = commData.cmdParDic[
                                    XyPtoPEnd.FolderparKey_requestfile];

                            totleSendFileLength = new FileInfo(sendFileName).Length;

                            commResult.resultDataDic.Add(
                                FolderparKey_filelength, totleSendFileLength.ToString());
                            commResult.cmdSucceed = true;

                            _xyPtoPFileEventHandler(
                                this,
                                new XyCommFileEventArgs(
                                    XyCommFileSendReceive.Send,
                                    totleSendFileLength,
                                    0
                                    )
                                );

                            //start send file task
                            string sendfile =
                                commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfile];
                            _ = sendFileAsync(sendfile);

                            break;
                        case XyPtoPCmd.ActiveSendFile:
                            _xyPtoPRequestHandler(commData, commResult);
                            string localFileName
                                = commData.cmdParDic[
                                    XyPtoPEnd.FolderparKey_sendfile];
                            
                            //ready to receive file
                            ReceiveFile(localFileName);

                            totleSendFileLength = long.Parse(
                                commData.cmdParDic[FolderparKey_filelength]);
                            _xyPtoPFileEventHandler(
                                this,
                                new XyCommFileEventArgs(
                                    XyCommFileSendReceive.Receive,
                                    totleSendFileLength,
                                    0)
                                );

                            break;

                        default:
                            commResult.resultDataDic.Add(
                                CommResult.resultDataKey_ErrorInfo,
                                "Cannot hand this command");
                            commResult.cmdSucceed = true;
                            break;
                    }

                    myIXyComm.send(commResult.toCommDic());
                }
                else
                {
                    //is Reply. set sendDataResult to not sendData function
                    sendDataResult = CommResult.fromReceivedResult(parsDic);
                }
            }
            else
            {
                if (receivedByteArrs != null)
                {
                    int receivedNum = e.ReceivedBytes.Length - 8;

                    byte[] pkgID = new byte[8];
                    pkgID[0] = e.ReceivedBytes[receivedNum - 8];
                    pkgID[1] = e.ReceivedBytes[receivedNum - 7];
                    pkgID[2] = e.ReceivedBytes[receivedNum - 6];
                    pkgID[3] = e.ReceivedBytes[receivedNum - 5];
                    pkgID[4] = e.ReceivedBytes[receivedNum - 4];
                    pkgID[5] = e.ReceivedBytes[receivedNum - 3];
                    pkgID[6] = e.ReceivedBytes[receivedNum - 2];
                    pkgID[7] = e.ReceivedBytes[receivedNum - 1];

                    Int64 pkgIDNumber = BitConverter.ToInt64(pkgID);

                    lock (receivedByteArrs)
                    {
                        if (pkgIDNumber >= WaitedPkgID
                            && !receivedByteArrs.ContainsKey(pkgIDNumber))
                        {
                            receivedByteArrs.Add(pkgIDNumber, e.ReceivedBytes);
                        }
                    }
                }
            }

        }

        public void clean()
        {
            if (myIXyComm != null)
            {
                myIXyComm.clean();
            }
        }

        #region 命令

        private int sleepTime = 50;
        private CommResult? sendDataResult;
        public async Task<CommResult> sendData(CommData commData)
        {
            CommResult taskResult =  await Task.Run(
                () => {
                    sendDataResult = null;
                    myIXyComm.send(commData.toCommDic());

                    while (true)
                    {
                        if(sendDataResult != null)
                        {
                            //检查是否配置的响应                            
                            if(sendDataResult.cmdID != commData.cmdID)
                            {
                                sendDataResult.errorCmdID = true;
                            }

                            break;
                        }

                        //超时检查

                        Thread.Sleep(sleepTime);
                    }

                    return sendDataResult;
                });

            return taskResult;
        }

        private long receivedSentBytes = 0;
        private async Task sendFileAsync(string sendfile)
        {
            int maxBufferTaskCount = 50;
            int maxSendTaskCount = 30;

            List<fileSendTask> sendTaskDataList
                = new List<fileSendTask>();
            Dictionary<Int64, fileSendTask> inSendTaskDataDic 
                = new Dictionary<long, fileSendTask>();

            Dictionary<Int64, Task> sendTasksDic 
                = new Dictionary<Int64, Task>();

            receivedSentBytes = 0;

            await Task.Run(
            async Task () =>
            {
                try
                {
                    int sendLength = 1024 * 32;
                    int bufferSize = sendLength + 8 + 8; //+sendID+sendLength
                    byte[] filechunk;
                    int numBytes;
                    long sentBytes = 0;

                    FileStream file = new FileStream(sendfile, FileMode.Open);

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
                                    file.Read(filechunk, 0, sendLength);
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
                                        await myIXyComm.sendNumberedStream(
                                            fst.SendBytes,
                                            fst.SendLength,
                                            fst.SendNumber
                                            );
                                    }
                                    catch (Exception e)
                                    {
                                        d("myIXyComm.sendNumberedStream: "
                                            + e.Message + "" + e.StackTrace);
                                    }
                                }
                                receivedSentBytes += fst.SendLength;
                                _xyPtoPFileEventHandler(
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

                            _xyPtoPFileEventHandler(
                                this,
                                new XyCommFileEventArgs(
                                    XyCommFileSendReceive.Send,
                                    totleSendFileLength,
                                    totleSendFileLength // why not equal receivedSentBytes？
                                    )
                                );

                            d("all file pkg sent: ");
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
        }

        #region 主控端命令

        static public string FolderparKey_folders = "folders";
        static public string FolderparKey_files = "files";
        static public string FolderparKey_hostName = "hostName";
        static public string FolderparKey_requestfolder = "requestfolder";
        static public string FolderparKey_requestfile = "requestfile";
        static public string FolderparKey_sendfile = "sendfile";
        static public string FolderparKey_filelength = "filelength";
        static public string FolderparKey_fileprogress = "fileprogress";
        public async Task<CommResult> ActiveGetInitFolder()
        {
            CommData commData = new CommData(XyPtoPCmd.ActiveGetInitFolder);

            return await sendData(commData);
        }
        public async Task<CommResult> ActiveGetFolder(string requestFolder)
        {
            CommData commData = new CommData(XyPtoPCmd.ActiveGetFolder);
            commData.cmdParDic.Add(
                FolderparKey_requestfolder, requestFolder);

            return await sendData(commData);
        }
        
        public async Task ActiveGetFile(
            string requestFile,
            string receivedFile)
        {
            CommData commData = new CommData(XyPtoPCmd.ActiveGetFile);
            commData.cmdParDic.Add(
                FolderparKey_requestfile, requestFile);

            //ready to receive file
            Task receiveFileTask = Task.Run(
                ()=> { 
                    ReceiveFile(receivedFile); 
                }
                );

            CommResult sendCommandResult = await sendData(commData);

            totleSendFileLength = long.Parse(
                sendCommandResult.resultDataDic[FolderparKey_filelength]);
            _xyPtoPFileEventHandler(
                this,
                new XyCommFileEventArgs(
                    XyCommFileSendReceive.Receive,
                    totleSendFileLength, 
                    0)
                );

            await receiveFileTask;
        }

        long receviedLength = 0;
        long totleSendFileLength = 0;

        private Dictionary<Int64, byte[]> receivedByteArrs;
        Int64 WaitedPkgID = 0;
        private void ReceiveFile(string receivedFile)
        {
            string receivedFileName = receivedFile;
            receviedLength = 0;
            totleSendFileLength = -1;

            receivedByteArrs = new Dictionary<long, byte[]>();

            WaitedPkgID = 0;

            FileStream receiveFileStream = new FileStream(
                receivedFileName, 
                FileMode.Create, 
                FileAccess.Write);

            _ = Task.Run(() => {
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

                                _xyPtoPFileEventHandler(
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

                receivedFileName = null;
                if (receiveFileStream != null)
                {
                    receiveFileStream.Close();
                }
                receviedLength = 0;
                WaitedPkgID = 0;
                totleSendFileLength = 0;
            });
        }

        public async Task ActiveSendFile(
            string localFile,
            string remoteFile)
        {
            CommData commData = new CommData(XyPtoPCmd.ActiveSendFile);
            commData.cmdParDic.Add(
                FolderparKey_sendfile, remoteFile);

            totleSendFileLength = new FileInfo(localFile).Length;
            commData.cmdParDic.Add(
                FolderparKey_filelength, totleSendFileLength.ToString());

            _xyPtoPFileEventHandler(
                this,
                new XyCommFileEventArgs(
                    XyCommFileSendReceive.Send,
                    totleSendFileLength,
                    0)
                );

            CommResult sendCommandResult = await sendData(commData);

            //start send file task
            await sendFileAsync(localFile);
        }

        #endregion

        #region 被控端命令

        public async Task<CommResult> Regist(
            Dictionary<string, string> pLocalEndPars,
            Dictionary<string, string> pRemoteEndPars
            )
        {
            myIXyComm.setTargetEndPoint(pRemoteEndPars);

            CommData commData = new CommData(XyPtoPCmd.PassiveRegist);
            commData.cmdParDic = pLocalEndPars;

            return await sendData(commData);
        }

        #endregion

        #endregion

        //debug
        static public void d(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }

    public delegate void
        XyPtoPRequestHandler(CommData commData, CommResult commResult);
    public enum XyPtoPEndType { ActiveEnd, PassiveEnd }
    public enum XyPtoPCmd {
        PassiveRegist,
        ActiveGetInitFolder,
        ActiveGetFolder,
        ActiveGetFile,
        ActiveSendFile
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
