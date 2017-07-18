using CSProtocol;
using System;

[CheatCommand("工具/ProfitLimit", "重置收益上限", 0x52)]
internal class ProfitLimitCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CheatCmdRef.stClrProfitLimit = new CSDT_CHEAT_COMVAL();
        CheatCmdRef.stClrProfitLimit.iValue = 1;
        return CheatCommandBase.Done;
    }
}

