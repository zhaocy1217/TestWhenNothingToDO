using System;

[CheatCommand("Fow/ToggleDebugVisibleCell", "开关可见格", 0)]
internal class CheatCommandToggleDebugVisibleCell : CheatCommandCommon
{
    protected override string Execute(string[] InArguments)
    {
        FogOfWarWrapper.ToggleDebugVisibleCell();
        return CheatCommandBase.Done;
    }
}

