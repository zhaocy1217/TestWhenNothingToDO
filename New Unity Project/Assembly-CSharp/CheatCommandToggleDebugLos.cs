using System;

[CheatCommand("Fow/ToggleDebugLos", "开关视线", 0)]
internal class CheatCommandToggleDebugLos : CheatCommandCommon
{
    protected override string Execute(string[] InArguments)
    {
        FogOfWarWrapper.ToggleDebugLos();
        return CheatCommandBase.Done;
    }
}

