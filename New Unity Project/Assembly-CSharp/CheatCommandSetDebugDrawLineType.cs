using System;

[ArgumentDescription(0, typeof(int), "inDrawLineType", new object[] {  }), CheatCommand("Fow/SetDebugDrawLineType", "设置视线调试阶段", 0)]
internal class CheatCommandSetDebugDrawLineType : CheatCommandCommon
{
    protected override string Execute(string[] InArguments)
    {
        FogOfWarWrapper.SetDebugDrawLineType(CheatCommandBase.SmartConvert<int>(InArguments[0]));
        return CheatCommandBase.Done;
    }
}

