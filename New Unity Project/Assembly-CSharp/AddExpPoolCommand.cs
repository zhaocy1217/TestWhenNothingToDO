using CSProtocol;
using System;

[CheatCommand("英雄/属性修改/经验等级/AddExpPool", "加经验池", 15), ArgumentDescription(typeof(int), "数量", new object[] {  })]
internal class AddExpPoolCommand : CommonValueChangeCommand
{
    protected override void FillMessageField(ref CSDT_CHEATCMD_DETAIL CheatCmdRef, int InValue)
    {
        CheatCmdRef.stUpdAcntInfo = new CSDT_CHEAT_UPDACNTINFO();
        CheatCmdRef.stUpdAcntInfo.iUpdType = 15;
        CheatCmdRef.stUpdAcntInfo.iUpdValue = InValue;
    }
}

