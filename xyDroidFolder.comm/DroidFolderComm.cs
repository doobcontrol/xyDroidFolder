using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using xySoft.comm;

namespace xyDroidFolder.comm
{
    public class DroidFolderComm
    {
        private IXyComm myIXyComm;
        private DroidFolderRequestHandler _droidFolderRequestHandler;
        event EventHandler<XyFileIOEventArgs> _xyFileIOEventHandler;

        public DroidFolderComm(
            string localIp, int localPort,
            string targetIp, int targetPort,
            DroidFolderRequestHandler droidFolderRequestHandler,
            EventHandler<XyFileIOEventArgs> xyFileIOEventHandler
            )
        {
            myIXyComm = new XyUdpComm(
                localIp, localPort,
                targetIp, targetPort,
                XyCommRequestHandler);
            _droidFolderRequestHandler = droidFolderRequestHandler;
            _xyFileIOEventHandler = xyFileIOEventHandler;
            myIXyComm.startListen();
        }
        public void clean()
        {
            if (myIXyComm != null)
            {
                myIXyComm.stopListen();
            }
        }

        private string XyCommRequestHandler(string receivedString)
        {
            CommResult commResult;

            try
            {
                CommData commData = CommData.fromReceivedString(receivedString);
                commResult = new CommResult(commData);
                commResult.cmdSucceed = true;

                switch (commData.cmd)
                {
                    case DroidFolderCmd.Register:
                        //register target ip and port
                        Dictionary<string, string> setDic = new Dictionary<string, string>();
                        setDic[XyUdpCommTargetSetPar.ip.ToString()]
                            = commData.cmdParDic[CmdPar.ip.ToString()];
                        setDic[XyUdpCommTargetSetPar.port.ToString()]
                            = commData.cmdParDic[CmdPar.port.ToString()];
                        myIXyComm.set(setDic);

                        _droidFolderRequestHandler(commData, commResult);

                        commResult.resultDataDic.Add(
                            CmdPar.returnMsg.ToString(),
                            "Regist ok");
                        break;
                    case DroidFolderCmd.GetInitFolder:
                        _droidFolderRequestHandler(commData, commResult);
                        break;
                    case DroidFolderCmd.GetFolder:
                        _droidFolderRequestHandler(commData, commResult);
                        break;
                    case DroidFolderCmd.GetFile:
                        _droidFolderRequestHandler(commData, commResult);

                        _xyFileIOEventHandler(
                            this,
                            new XyFileIOEventArgs(
                                FileIOEventType.start,
                                XyCommFileSendReceive.Send,
                                int.Parse(commResult.resultDataDic[CmdPar.fileLength.ToString()]),
                                0
                                )
                            );

                        _ = myIXyComm.sendFile(
                            commData.cmdParDic[CmdPar.targetFile.ToString()],
                            commResult.resultDataDic[CmdPar.fileLength.ToString()],
                            commData.cmdParDic[CmdPar.streamReceiverPar.ToString()],
                            FileEventHandler);
                        break;
                    case DroidFolderCmd.SendFile:
                        _droidFolderRequestHandler(commData, commResult);

                        _xyFileIOEventHandler(
                            this,
                            new XyFileIOEventArgs(
                                FileIOEventType.start,
                                XyCommFileSendReceive.Receive,
                                int.Parse(commData.cmdParDic[CmdPar.fileLength.ToString()]),
                                0
                                )
                            );

                        _ = myIXyComm.prepareStreamReceiver(
                            commData.cmdParDic[CmdPar.targetFile.ToString()],
                            commData.cmdParDic[CmdPar.fileLength.ToString()],
                            commResult.resultDataDic[CmdPar.streamReceiverPar.ToString()],
                            FileEventHandler);

                        break;
                    case DroidFolderCmd.SendText:
                        commData.cmdParDic[CmdPar.text.ToString()] = 
                            decodeParString(commData.cmdParDic[CmdPar.text.ToString()]);
                        _droidFolderRequestHandler(commData, commResult);
                        break;

                    default:
                        commResult.resultDataDic.Add(
                            CmdPar.errorMsg.ToString(),
                            "Cannot hand this command");
                        commResult.cmdSucceed = false;
                        break;
                }
            }
            catch(Exception e)
            {
                commResult = null;
            }

            string retStr = null;
            if (commResult != null)
            {
                retStr = commResult.toSendString();
            }
            return retStr;
        }

        private void FileEventHandler(object? sender, XyCommFileEventArgs e)
        {
            _xyFileIOEventHandler(
                this,
                new XyFileIOEventArgs(
                    (e.Length > e.Progress)?FileIOEventType.progress:
                        FileIOEventType.end,
                    e.FileSendReceive,
                    e.Length,
                    e.Progress
                    )
                );
        }

        private async Task<CommResult> XyCommRequestAsync(CommData commData)
        {
            string resultString = null;

            try
            {
                resultString = await myIXyComm.sendForResponseAsync(
                    commData.toSendString()
                    );
            }
            catch(XySoftCommException xse)
            {
                switch (xse.ErrorCode)
                {
                    case XyCommErrorCode.TimedOut:
                        throw new DroidFolderCommException(
                            DroidFolderCommErrorCode.TimedOut,
                            "TimedOut");
                    default:
                        throw new DroidFolderCommException(
                            DroidFolderCommErrorCode.OtherError,
                            "Net work error");
                }
            }

            return CommResult.fromReturnString(resultString, commData);
        }

        public async Task<CommResult> Register(
            string ip, int port, string hostName
            )
        {
            CommData commData = new CommData(DroidFolderCmd.Register);
            commData.cmdParDic.Add(CmdPar.ip.ToString(), ip);
            commData.cmdParDic.Add(CmdPar.port.ToString(), port.ToString());
            commData.cmdParDic.Add(CmdPar.hostName.ToString(), hostName);

            return await XyCommRequestAsync(commData);
        }

        public async Task<CommResult> GetInitFolder()
        {
            CommData commData = new CommData(DroidFolderCmd.GetInitFolder);

            CommResult commResult = await XyCommRequestAsync(commData);

            return commResult;
        }

        public async Task<CommResult> GetFolder(string path)
        {
            CommData commData = new CommData(DroidFolderCmd.GetFolder);
            commData.cmdParDic.Add(CmdPar.requestPath.ToString(), path);

            return await XyCommRequestAsync(commData);
        }

        public async Task GetFile(
            string receiveFile, 
            string targetFile,
            string streamReceiverPar
            )
        {
            CommData commData = new CommData(DroidFolderCmd.GetFile);
            commData.cmdParDic.Add(CmdPar.targetFile.ToString(), targetFile);
            commData.cmdParDic.Add(CmdPar.streamReceiverPar.ToString(), 
                streamReceiverPar);

            CommResult commResult = await XyCommRequestAsync(commData);

            _xyFileIOEventHandler(
                this,
                new XyFileIOEventArgs(
                    FileIOEventType.start,
                    XyCommFileSendReceive.Receive,
                    int.Parse(commResult.resultDataDic[CmdPar.fileLength.ToString()]),
                    0
                    )
                );

            await myIXyComm.prepareStreamReceiver(
                receiveFile,
                commResult.resultDataDic[CmdPar.fileLength.ToString()],
                streamReceiverPar,
                FileEventHandler);

            _xyFileIOEventHandler(
                this,
                new XyFileIOEventArgs(
                    FileIOEventType.end,
                    XyCommFileSendReceive.Receive,
                    0,
                    0
                    )
                );
        }

        public async Task SendFile(string sendFile, string targetFile)
        {
            string fileLengthStr = new FileInfo(sendFile).Length.ToString();
            CommData commData = new CommData(DroidFolderCmd.SendFile);
            commData.cmdParDic.Add(CmdPar.targetFile.ToString(), targetFile);
            commData.cmdParDic.Add(CmdPar.fileLength.ToString(), 
                new FileInfo(sendFile).Length.ToString());

            d("request send file ...");
            CommResult commResult = await XyCommRequestAsync(commData);
            d("request confirmed, start send ...");

            _xyFileIOEventHandler(
                this,
                new XyFileIOEventArgs(
                    FileIOEventType.start,
                    XyCommFileSendReceive.Send,
                    int.Parse(fileLengthStr),
                    0
                    )
                );

            await myIXyComm.sendFile(
                sendFile,
                fileLengthStr,
                commResult.resultDataDic[CmdPar.streamReceiverPar.ToString()],
                FileEventHandler);
            d("send done");

            _xyFileIOEventHandler(
                this,
                new XyFileIOEventArgs(
                    FileIOEventType.end,
                    XyCommFileSendReceive.Send,
                    0,
                    0
                    )
                );
        }
       
        public async Task<CommResult> SendText(string sendText)
        {
            CommData commData = new CommData(DroidFolderCmd.SendText);
            commData.cmdParDic.Add(CmdPar.text.ToString(), encodeParString(sendText));

            return await XyCommRequestAsync(commData);
        }

        #region encode/decode par string

        static Dictionary<string, string> encodeDic = new Dictionary<string, string>()
        {
            {",", "xyCommA" },
            { "=", "xyEquaL" }
        };

        static Dictionary<string, string> dncodeDic = new Dictionary<string, string>()
        {
            {"xyCommA", "," },
            { "xyEquaL", "=" }
        };

        static public string encodeParString(string parString)
        {
            return stringReplace(parString, encodeDic);
        }
        static public string decodeParString(string parString)
        {
            return stringReplace(parString, dncodeDic);
        }
        static public string stringReplace(string rString, 
            Dictionary<string, string> rDic)
        {
            string retString = rString;

            foreach (string key in rDic.Keys)
            {
                retString = retString.Replace(key, rDic[key]);
            }

            return retString;
        }

        #endregion

        //debug
        static public void d(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
    public enum DroidFolderCmd
    {
        Register,
        GetInitFolder,
        GetFolder,
        GetFile,
        SendFile,
        SendText
    }
    public enum CmdPar
    {
        cmd,
        cmdID,
        cmdSucceed,
        ip,
        port,
        hostName,
        requestPath,
        targetFile,
        text,
        fileLength,
        streamReceiverPar,
        folders,
        files,
        returnMsg,
        errorMsg
    }
    public enum DroidFolderCommErrorCode
    {
        TimedOut = 10060,
        OtherError = 0
    }

    public delegate void
        DroidFolderRequestHandler(CommData commData, CommResult commResult);

    public class XyFileIOEventArgs : EventArgs
    {
        public XyFileIOEventArgs(
            FileIOEventType type,
            XyCommFileSendReceive fileSendReceive,
            long length,
            long progress)
        {
            this.FileSendReceive = fileSendReceive;
            Length = length;
            Progress = progress;
            Type = type;
        }

        public XyCommFileSendReceive FileSendReceive { get; private set; }
        public long Length { get; private set; }
        public long Progress { get; private set; }
        public FileIOEventType Type { get; private set; }
    }
    public enum FileIOEventType
    {
        start,
        end,
        progress
    }
}
