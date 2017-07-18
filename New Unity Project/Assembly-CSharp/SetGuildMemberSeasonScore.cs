using CSProtocol;
using System;

[ArgumentDescription(typeof(int), "个人赛季积分", new object[] {  }), CheatCommand("通用/战队/SetGuildMemberSeasonScore", "设置个人战队赛赛季积分(需要重登陆)", 0x3f)]
internal class SetGuildMemberSeasonScore : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stSetGuildMemberInfo = new CSDT_CHEAT_SET_GUILD_MEMBER_INFO();
        CheatCmdRef.stSetGuildMemberInfo.iGuildCoin = -1;
        CheatCmdRef.stSetGuildMemberInfo.iGuildMatchScore = InValue;
        CheatCmdRef.stSetGuildMemberInfo.iGuildMatchWeekScore = -1;
        CheatCmdRef.stSetGuildMemberInfo.iContinueWin = -1;
        CheatCmdRef.stSetGuildMemberInfo.iWeekMatchCnt = -1;
    }
}

