namespace Assets.Scripts.Framework
{
    using CSProtocol;
    using System;
    using System.Net;
    using UnityEngine;

    public class ReconnectIpSelect : Singleton<ReconnectIpSelect>
    {
        private NetworkReachability lobbylastSucNetworkReachability;
        private string m_curLobbyUrlForNormal;
        private string m_curLobbyUrlForTongcai;
        private string m_curRelayUrlForNormal;
        private string m_curRelayUrlForTongcai;
        private string m_successLobbyUrlForNormal;
        private string m_successLobbyUrlForTongcai;
        private string m_successRelayUrlForNormal;
        private string m_successRelayUrlForTongcai;
        private COMDT_TGWINFO m_tgwInfo;
        private NetworkReachability relaylastSucNetworkReachability;
        private ListView<string> tempIpList = new ListView<string>();

        public string GetConnectUrl(ConnectorType connectType, uint curConnectTime)
        {
            if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
            {
                if (connectType == ConnectorType.Lobby)
                {
                    if (!string.IsNullOrEmpty(this.m_successLobbyUrlForTongcai) && (this.lobbylastSucNetworkReachability == Application.get_internetReachability()))
                    {
                        return this.m_successLobbyUrlForTongcai;
                    }
                    this.m_curLobbyUrlForTongcai = this.GetLobbyTongcaiConnectUrl(curConnectTime);
                    return this.m_curLobbyUrlForTongcai;
                }
                if (!string.IsNullOrEmpty(this.m_successRelayUrlForTongcai) && (this.relaylastSucNetworkReachability == Application.get_internetReachability()))
                {
                    return this.m_successRelayUrlForTongcai;
                }
                this.m_curRelayUrlForTongcai = this.GetRelayTongCaiConnectUrl(curConnectTime);
                return this.m_curRelayUrlForTongcai;
            }
            if (connectType == ConnectorType.Lobby)
            {
                if (!string.IsNullOrEmpty(this.m_successLobbyUrlForNormal) && (this.lobbylastSucNetworkReachability == Application.get_internetReachability()))
                {
                    return this.m_successLobbyUrlForNormal;
                }
                this.m_curLobbyUrlForNormal = this.GetLobbyNormalConnectUrl(curConnectTime);
                return this.m_curLobbyUrlForNormal;
            }
            if (!string.IsNullOrEmpty(this.m_successRelayUrlForNormal) && (this.relaylastSucNetworkReachability == Application.get_internetReachability()))
            {
                return this.m_successRelayUrlForNormal;
            }
            this.m_curRelayUrlForNormal = this.GetRelayNormalConnectUrl(curConnectTime);
            return this.m_curRelayUrlForNormal;
        }

        private string GetIPFromTgw(COMDT_TGWINFO tgwInfo, IspType ispType)
        {
            this.tempIpList.Clear();
            for (int i = 0; i < tgwInfo.dwVipCnt; i++)
            {
                if (tgwInfo.astVipInfo[i].iISPType == ispType)
                {
                    string item = new IPAddress((long) tgwInfo.astVipInfo[i].dwVIP).ToString();
                    this.tempIpList.Add(item);
                }
            }
            if (this.tempIpList.Count == 0)
            {
                return null;
            }
            int num2 = Random.Range(0, this.tempIpList.Count);
            return this.tempIpList[num2];
        }

        private string GetLobbyNormalConnectUrl(uint curConnectTime)
        {
            string ip = null;
            int connectIndex = MonoSingleton<TdirMgr>.instance.m_connectIndex;
            if ((curConnectTime == 1) || (curConnectTime == 2))
            {
                IPAddrInfo info = MonoSingleton<TdirMgr>.instance.SelectedTdir.addrs[connectIndex];
                ip = info.ip;
            }
            else if ((curConnectTime == 3) || (curConnectTime == 4))
            {
                ip = MonoSingleton<TdirMgr>.instance.GetLianTongIP();
            }
            else if ((curConnectTime == 5) || (curConnectTime == 6))
            {
                ip = MonoSingleton<TdirMgr>.instance.GetYiDongIP();
            }
            else if ((curConnectTime == 7) || (curConnectTime == 8))
            {
                ip = MonoSingleton<TdirMgr>.instance.GetDianXingIP();
            }
            if (ip == null)
            {
                IPAddrInfo info2 = MonoSingleton<TdirMgr>.instance.SelectedTdir.addrs[connectIndex];
                ip = info2.ip;
            }
            return ip;
        }

        private string GetLobbyTongcaiConnectUrl(uint curConnectTime)
        {
            string tongcaiUrl = null;
            if ((curConnectTime == 1) || (curConnectTime == 2))
            {
                tongcaiUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
            }
            else if ((curConnectTime == 3) || (curConnectTime == 4))
            {
                tongcaiUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[2];
            }
            else if ((curConnectTime == 5) || (curConnectTime == 6))
            {
                tongcaiUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[3];
            }
            else if ((curConnectTime == 7) || (curConnectTime == 8))
            {
                tongcaiUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiIps[1];
            }
            if (tongcaiUrl == null)
            {
                tongcaiUrl = MonoSingleton<CTongCaiSys>.instance.TongcaiUrl;
            }
            return tongcaiUrl;
        }

        private string GetRelayNormalConnectUrl(uint curConnectTime)
        {
            string loginOnlyIpOrUrl = null;
            int connectIndex = MonoSingleton<TdirMgr>.instance.m_connectIndex;
            if ((curConnectTime == 1) || (curConnectTime == 2))
            {
                if ((this.m_tgwInfo.szRelayUrl.Length > 0) && (this.m_tgwInfo.szRelayUrl[0] != 0))
                {
                    loginOnlyIpOrUrl = StringHelper.UTF8BytesToString(ref this.m_tgwInfo.szRelayUrl);
                }
                else
                {
                    loginOnlyIpOrUrl = ApolloConfig.loginOnlyIpOrUrl;
                }
            }
            else if ((curConnectTime == 3) || (curConnectTime == 4))
            {
                loginOnlyIpOrUrl = this.GetIPFromTgw(this.m_tgwInfo, IspType.Liantong);
            }
            else if ((curConnectTime == 5) || (curConnectTime == 6))
            {
                loginOnlyIpOrUrl = this.GetIPFromTgw(this.m_tgwInfo, IspType.Yidong);
            }
            else if ((curConnectTime == 7) || (curConnectTime == 8))
            {
                loginOnlyIpOrUrl = this.GetIPFromTgw(this.m_tgwInfo, IspType.Dianxing);
            }
            if (loginOnlyIpOrUrl != null)
            {
                return loginOnlyIpOrUrl;
            }
            if ((this.m_tgwInfo.szRelayUrl.Length > 0) && (this.m_tgwInfo.szRelayUrl[0] != 0))
            {
                return StringHelper.UTF8BytesToString(ref this.m_tgwInfo.szRelayUrl);
            }
            return ApolloConfig.loginOnlyIpOrUrl;
        }

        private string GetRelayTongCaiConnectUrl(uint curConnectTime)
        {
            return this.GetLobbyTongcaiConnectUrl(curConnectTime);
        }

        public void Reset()
        {
            this.m_successLobbyUrlForTongcai = null;
            this.m_successLobbyUrlForNormal = null;
            this.m_successRelayUrlForTongcai = null;
            this.m_successRelayUrlForNormal = null;
            this.lobbylastSucNetworkReachability = 0;
            this.relaylastSucNetworkReachability = 0;
        }

        public void SetLobbySuccessUrl(string url)
        {
            this.lobbylastSucNetworkReachability = Application.get_internetReachability();
            if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
            {
                if (this.m_successLobbyUrlForTongcai == null)
                {
                    this.m_successLobbyUrlForTongcai = url;
                }
            }
            else if (this.m_successLobbyUrlForNormal == null)
            {
                this.m_successLobbyUrlForNormal = url;
            }
            this.m_curLobbyUrlForTongcai = null;
            this.m_curLobbyUrlForNormal = null;
        }

        public void SetRelaySuccessUrl(string url)
        {
            this.relaylastSucNetworkReachability = Application.get_internetReachability();
            if (MonoSingleton<CTongCaiSys>.instance.IsCanUseTongCai())
            {
                if (this.m_successRelayUrlForTongcai == null)
                {
                    this.m_successRelayUrlForTongcai = url;
                }
            }
            else if (this.m_successRelayUrlForNormal == null)
            {
                this.m_successRelayUrlForNormal = url;
            }
            this.m_curRelayUrlForTongcai = null;
            this.m_curRelayUrlForNormal = null;
        }

        public void SetRelayTgw(COMDT_TGWINFO tgw)
        {
            this.m_tgwInfo = tgw;
            this.m_successRelayUrlForNormal = null;
        }
    }
}

