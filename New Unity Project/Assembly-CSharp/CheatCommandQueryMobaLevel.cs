using CSProtocol;
using System;

[CheatCommand("关卡/QueryMobaInfo", "查询mobalevel", 80), ArgumentDescription(0, typeof(string), "OpenId", new object[] {  })]
internal class CheatCommandQueryMobaLevel : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        CheatCmdRef.stGetMobaInfo = new CSDT_CHEAT_GET_MOBA_INFO();
        StringHelper.StringToUTF8Bytes(InArguments[0], ref CheatCmdRef.stGetMobaInfo.szOpenID);
        return CheatCommandBase.Done;
    }
}

