namespace Assets.Scripts.Framework
{
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using CSProtocol;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [MessageHandlerClass]
    public class SynchrReport
    {
        private static uint _checkFrameNo;
        private static bool _isAlerted;
        private static bool _isDeskUnsync;
        private static bool _isSelfUnsync;
        private static bool _isUploading;
        private static int _uploadIndex = -1;
        private static List<MemoryStream> _uploadList;

        private static void CloseUpload()
        {
            if (!_isUploading)
            {
                if (_uploadList != null)
                {
                    for (int i = 0; i < _uploadList.Count; i++)
                    {
                        if (_uploadList[i] != null)
                        {
                            _uploadList[i].Close();
                        }
                    }
                    _uploadList = null;
                }
                _uploadIndex = -1;
                if (_isSelfUnsync && !_isAlerted)
                {
                    _isAlerted = true;
                    if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("MultiGame_Not_Sync"), enUIEventID.Lobby_ConfirmErrExit, false);
                    }
                    else
                    {
                        Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("MultiGame_Not_Sync_Try"), enUIEventID.Battle_MultiHashInvalid, false);
                    }
                    DebugHelper.CustomLog("HashCheckInvalid!");
                    BuglyAgent.ReportException(new LobbyMsgHandler.HashCheckInvalide("HaskCheckInvalide"), "MultiGame not synced!");
                }
            }
        }

        [MessageHandler(0x504)]
        public static void OnHashCheckRsp(CSPkg pkg)
        {
            _isSelfUnsync |= pkg.stPkgData.stRelayHashChkRsp.dwIsSelfNE != 0;
            _isDeskUnsync |= pkg.stPkgData.stRelayHashChkRsp.dwIsDeskNE != 0;
            CloseUpload();
        }

        [MessageHandler(0x1476)]
        public static void OnUpload(CSPkg pkg)
        {
            Upload((long) pkg.stPkgData.stUploadCltlogReq.dwOffset);
        }

        public static void Reset()
        {
            _uploadList = null;
            _uploadIndex = -1;
            _isUploading = false;
            _checkFrameNo = 0;
            _isSelfUnsync = false;
            _isDeskUnsync = false;
            _isAlerted = false;
        }

        private static void Upload(long offset)
        {
            CSPkg msg = NetworkModule.CreateDefaultCSPKG(0x1477);
            CSPKG_UPLOADCLTLOG_NTF stUploadCltlogNtf = msg.stPkgData.stUploadCltlogNtf;
            stUploadCltlogNtf.dwLogType = 0;
            bool flag = false;
            while (((_uploadList != null) && (_uploadIndex >= 0)) && ((_uploadIndex < _uploadList.Count) && (_uploadList[_uploadIndex] == null)))
            {
                _uploadIndex++;
            }
            if (((_uploadList != null) && (_uploadIndex >= 0)) && (_uploadIndex < _uploadList.Count))
            {
                MemoryStream stream = _uploadList[_uploadIndex];
                if (offset < stream.Length)
                {
                    if (offset != stream.Position)
                    {
                        stream.Position = offset;
                    }
                    stUploadCltlogNtf.dwLogType = ((uint) ((_uploadIndex + 1) * 0x989680)) + _checkFrameNo;
                    stUploadCltlogNtf.dwBuffOffset = (uint) offset;
                    stUploadCltlogNtf.dwBufLen = (uint) stream.Read(stUploadCltlogNtf.szBuf, 0, stUploadCltlogNtf.szBuf.Length);
                    if (stream.Position >= stream.Length)
                    {
                        flag = ++_uploadIndex >= _uploadList.Count;
                        stUploadCltlogNtf.bThisLogOver = 1;
                        stUploadCltlogNtf.bAllLogOver = !flag ? ((byte) 0) : ((byte) 1);
                    }
                    else
                    {
                        stUploadCltlogNtf.bThisLogOver = 0;
                        stUploadCltlogNtf.bAllLogOver = 0;
                    }
                }
            }
            Singleton<NetworkModule>.GetInstance().SendGameMsg(ref msg, 0);
            if (flag || (stUploadCltlogNtf.dwLogType == 0))
            {
                _isUploading = false;
                CloseUpload();
            }
        }
    }
}

