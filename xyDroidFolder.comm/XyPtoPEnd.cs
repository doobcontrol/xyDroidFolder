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
                            sendFile(commData);

                            break;

                        //file send confim
                        case XyPtoPCmd.FileReceivedConfim:
                            receivedSentBytes = long.Parse(
                                commData.cmdParDic[
                                    XyPtoPEnd.FolderparKey_fileprogress]);

                            _xyPtoPFileEventHandler(
                                this,
                                new XyCommFileEventArgs(
                                    XyCommFileSendReceive.Send,
                                    totleSendFileLength,
                                    receivedSentBytes
                                    )
                                );

                            break;

                        default:
                            commResult.resultDataDic.Add(
                                CommResult.resultDataKey_ErrorInfo,
                                "Cannot hand this command");
                            commResult.cmdSucceed = true;
                            break;
                    }

                    sendResult(commResult);
                }
                else
                {
                    //is Reply. set sendDataResult to not sendData function
                    sendDataResult = CommResult.fromReceivedResult(parsDic);
                }
            }
            else
            {
                if (inFileReceive)
                {
                    int receivedNum = e.ReceivedBytes.Length;
                    receviedLength += receivedNum;
                    receiveFileStream.Write(
                        e.ReceivedBytes, 
                        0,
                        receivedNum);

                    _xyPtoPFileEventHandler(
                        this,
                        new XyCommFileEventArgs(
                            XyCommFileSendReceive.Receive,
                            totleSendFileLength,
                            receviedLength
                            )
                        );

                    CommData fileReceivedCommData
                        = new CommData(XyPtoPCmd.FileReceivedConfim);
                    fileReceivedCommData.cmdParDic.
                        Add(FolderparKey_fileprogress, receviedLength.ToString());

                    myIXyComm.send(fileReceivedCommData.toCommDic());

                    if(totleSendFileLength == receviedLength)
                    {
                        finishReceive();
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

        private void sendResult(CommResult commResult)
        {
            myIXyComm.send(commResult.toCommDic());
        }

        private long receivedSentBytes = 0;
        private void sendFile(CommData commData)
        {
            string sendfile = 
                commData.cmdParDic[XyPtoPEnd.FolderparKey_requestfile];
            receivedSentBytes = 0;
            _ = Task.Run(
            () =>
            {
                try
                {
                    int sendLength = 1024 * 32;
                    byte[] filechunk = new byte[sendLength];
                    int numBytes;
                    long sentBytes = 0;

                    FileStream file = new FileStream(sendfile, FileMode.Open);

                    while (true)
                    {
                        numBytes =
                            file.Read(filechunk, 0, sendLength);
                        if (numBytes > 0)
                        {
                            myIXyComm.sendStream(filechunk, numBytes);

                            //wait
                            sentBytes += numBytes;

                            while (true)
                            {
                                if (receivedSentBytes == sentBytes)
                                {
                                    break;
                                }

                                //超时检查

                                Thread.Sleep(sleepTime);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if(totleSendFileLength != receivedSentBytes)
                    {
                        //send error??
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

        #region 主控端命令

        static public string FolderparKey_folders = "folders";
        static public string FolderparKey_files = "files";
        static public string FolderparKey_hostName = "hostName";
        static public string FolderparKey_requestfolder = "requestfolder";
        static public string FolderparKey_requestfile = "requestfile";
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

            Dictionary<string, string>? sendPars
                = new Dictionary<string, string>();
            sendPars.Add("", receivedFile);

            //ready to receive file
            prepareReceive(commData, receivedFile);

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

            await new Task(() => { 
                
            });
        }

        bool inFileReceive = false;
        CommData? receivedFileCommData = null;
        string? receivedFileName = null;
        FileStream receiveFileStream;
        long receviedLength = 0;
        long totleSendFileLength = 0;
        private void prepareReceive(CommData commData, string receivedFile)
        {
            inFileReceive = true;
            receivedFileCommData = commData;
            receivedFileName = receivedFile;
            receviedLength = 0;
            totleSendFileLength = 0;

            receiveFileStream = new FileStream(
                receivedFileName, 
                FileMode.Create, 
                FileAccess.Write);
        }
        private void finishReceive()
        {

            inFileReceive = false;
            receivedFileCommData = null;
            receivedFileName = null;
            if (receiveFileStream != null)
            {
                receiveFileStream.Close();
            }
            receviedLength = 0;
            totleSendFileLength = 0;
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
    }

    public delegate void
        XyPtoPRequestHandler(CommData commData, CommResult commResult);
    public enum XyPtoPEndType { ActiveEnd, PassiveEnd }
    public enum XyPtoPCmd {
        PassiveRegist,
        ActiveGetInitFolder,
        ActiveGetFolder,
        ActiveGetFile,
        FileReceivedConfim
    }
}
