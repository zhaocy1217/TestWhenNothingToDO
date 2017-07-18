using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[MessageHandlerClass]
public class SCModuleControl : Singleton<SCModuleControl>
{
    private uint m_pvpAndPvpOffMask;
    private uint m_pvpAndPvpOffSec;
    private string m_pvpAndPvpOffTips = string.Empty;

    public void Clear()
    {
        this.m_pvpAndPvpOffMask = 0;
        this.m_pvpAndPvpOffSec = 0;
        this.m_pvpAndPvpOffTips = string.Empty;
    }

    public bool GetActiveModule(COM_CLIENT_PLAY_TYPE type)
    {
        return (((((COM_CLIENT_PLAY_TYPE) this.m_pvpAndPvpOffMask) & type) == COM_CLIENT_PLAY_TYPE.COM_CLIENT_PLAY_NULL) || (CRoleInfo.GetCurrentUTCTime() < this.m_pvpAndPvpOffSec));
    }

    public override void Init()
    {
        base.Init();
    }

    [MessageHandler(0x14ce)]
    public static void OnModuleSwitchNtf(CSPkg msg)
    {
        Singleton<SCModuleControl>.instance.OnModuleSwitchNtf(msg.stPkgData.stSwtichOffNtf.stOffNtfDetail);
    }

    public void OnModuleSwitchNtf(SCDT_NTF_SWITCHDETAIL msg)
    {
        this.m_pvpAndPvpOffMask = msg.dwOffMask;
        this.m_pvpAndPvpOffSec = msg.dwOffTime;
        this.m_pvpAndPvpOffTips = Utility.UTF8Convert(msg.szTips);
    }

    public override void UnInit()
    {
        base.UnInit();
    }

    public string PvpAndPvpOffTips
    {
        get
        {
            return this.m_pvpAndPvpOffTips;
        }
    }
}

