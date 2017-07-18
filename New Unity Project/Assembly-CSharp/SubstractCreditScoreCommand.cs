using CSProtocol;
using System;

[ArgumentDescription(1, typeof(int), "分数(负数才是减分)", new object[] {  }), ArgumentDescription(0, typeof(EComplaintType), "扣信誉积分类型", new object[] {  }), CheatCommand("英雄/属性修改/数值/SubstractCreditScore", "扣除信誉积分", 0x56)]
internal class SubstractCreditScoreCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        EComplaintType type = CheatCommandBase.SmartConvert<EComplaintType>(InArguments[0]);
        int num = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        CheatCmdRef.stDelCreditByType = new CSDT_CHEAT_DELCREDIT();
        CheatCmdRef.stDelCreditByType.dwType = (uint) type;
        CheatCmdRef.stDelCreditByType.iValue = num;
        return CheatCommandBase.Done;
    }
}

