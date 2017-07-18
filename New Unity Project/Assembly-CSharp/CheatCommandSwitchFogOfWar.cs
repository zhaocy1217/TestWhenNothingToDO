using System;

[CheatCommand("Fow/SwitchFogOfWar", "迷雾开关", 0)]
internal class CheatCommandSwitchFogOfWar : CheatCommandCommon
{
    protected override string Execute(string[] InArguments)
    {
        FogOfWarWrapper.enable = !FogOfWarWrapper.enable;
        return CheatCommandBase.Done;
    }
}

