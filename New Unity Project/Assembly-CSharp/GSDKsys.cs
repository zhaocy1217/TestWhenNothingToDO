using Apollo;
using com.tencent.gsdk;
using System;
using UnityEngine;

public class GSDKsys : MonoSingleton<GSDKsys>
{
    private bool m_bEnable;
    private string m_LastIP = string.Empty;
    private int m_LastPort;

    public void EndSpeed()
    {
        if (this.Enable)
        {
            GSDK.EndSpeed(this.m_LastIP, this.m_LastPort);
        }
    }

    public SpeedInfo GetSpeedInfo(string vip, int vport)
    {
        if (this.Enable)
        {
            return GSDK.GetSpeedInfo(vip, vport);
        }
        return new SpeedInfo();
    }

    public void InitSys()
    {
        if (!this.m_bEnable)
        {
            this.m_bEnable = false;
            if (!NetworkAccelerator.EnableForGSDK)
            {
                this.m_bEnable = true;
            }
            Debug.Log("HJJ init");
            if (this.m_bEnable)
            {
                GSDK.Init(ApolloConfig.appID, false);
                this.SetUserName();
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (this.Enable)
        {
            if (!pause)
            {
                GSDK.GoFront();
            }
            else
            {
                GSDK.GoBack();
            }
        }
    }

    public void SetUserName()
    {
        if (this.Enable)
        {
            ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
            if (accountInfo != null)
            {
                GSDK.SetUserName((int) ApolloConfig.platform, accountInfo.OpenId);
            }
        }
    }

    public void StartSpeed(string vip, int vport)
    {
        if (this.Enable)
        {
            this.m_LastIP = vip;
            this.m_LastPort = vport;
            GSDK.StartSpeed(vip, vport, 1, "libapollo.so", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID, 0);
        }
    }

    public bool Enable
    {
        get
        {
            return this.m_bEnable;
        }
    }
}

