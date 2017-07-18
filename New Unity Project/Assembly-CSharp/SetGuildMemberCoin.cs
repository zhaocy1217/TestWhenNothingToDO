using CSProtocol;
using System;

[CheatCommand("通用/战队/SetGuildMemberConstruct", "设置个人战队币", 0x3f), ArgumentDescription(typeof(int), "战队币", new object[] {  })]
internal class SetGuildMemberCoin : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetGuildMemberInfo = new CSDT_CHEAT_SET_GUILD_MEMBER_INFO();
        CheatCmdRef.stSetGuildMemberInfo.iGuildCoin = InValue;
        CheatCmdRef.stSetGuildMemberInfo.iGuildMatchScore = -1;
        CheatCmdRef.stSetGuildMemberInfo.iGuildMatchWeekScore = -1;
        CheatCmdRef.stSetGuildMemberInfo.iContinueWin = -1;
        CheatCmdRef.stSetGuildMemberInfo.iWeekMatchCnt = -1;
    }
}

