using CSProtocol;
using System;

[CheatCommand("关卡/SetNewbieGuideStateToOldPlayer", "成为老玩家", 0x1c)]
internal class SetNewbieGuideStateToOldPlayer : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        if ((Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() != null) && Singleton<LobbyLogic>.GetInstance().isLogin)
        {
            CheatCmdRef.stDyeNewbieBit = new CSDT_CHEAT_DYE_NEWBIE_BIT();
            CheatCmdRef.stDyeNewbieBit.bOpenOrClose = 1;
            CheatCmdRef.stDyeNewbieBit.bIsAll = 0;
            CheatCmdRef.stDyeNewbieBit.dwApntBit = 0;
            NewbieGuideManager.CompleteAllNewbieGuide();
            return CheatCommandBase.Done;
        }
        return "undone";
    }
}

