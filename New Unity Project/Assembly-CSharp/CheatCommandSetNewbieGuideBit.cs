using Assets.Scripts.GameSystem;
using System;

[ArgumentDescription(0, typeof(int), "引导位ID（弱引导位弱id+110）", new object[] {  }), ArgumentDescription(1, typeof(int), "开启或关闭(1置位0不置)", new object[] {  }), CheatCommand("关卡/CheatCommandSetNewbieGuideBit", "设置引导位", 0)]
internal class CheatCommandSetNewbieGuideBit : CheatCommandCommon
{
    protected override string Execute(string[] InArguments)
    {
        int inIndex = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        bool bOpen = CheatCommandBase.SmartConvert<int>(InArguments[1]) == 1;
        CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
        if ((masterRoleInfo != null) && Singleton<LobbyLogic>.GetInstance().isLogin)
        {
            if ((inIndex <= 0) || (inIndex >= 0x80))
            {
                return "错误的引导位ID";
            }
            masterRoleInfo.SetNewbieAchieve(inIndex, bOpen, true);
            MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieGuideState((uint) inIndex, bOpen);
        }
        return CheatCommandBase.Done;
    }
}

