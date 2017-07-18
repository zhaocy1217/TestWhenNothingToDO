using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Runtime.CompilerServices;

[ArgumentDescription(0, typeof(int), "章节", new object[] {  }), ArgumentDescription(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, 2, typeof(ELevelTypeTag), "章节", "普通", new object[] {  }), CheatCommand("关卡/UnlockLevel", "解锁闯关关卡", 12), ArgumentDescription(1, typeof(int), "序号", new object[] {  })]
internal class UnlockLevelCommand : CheatCommandNetworking
{
    protected override string Execute(string[] InArguments, ref CSDT_CHEATCMD_DETAIL CheatCmdRef)
    {
        <Execute>c__AnonStorey3C storeyc = new <Execute>c__AnonStorey3C();
        storeyc.Chapter = CheatCommandBase.SmartConvert<int>(InArguments[0]);
        storeyc.No = CheatCommandBase.SmartConvert<int>(InArguments[1]);
        ELevelTypeTag tag = CheatCommandBase.SmartConvert<ELevelTypeTag>(InArguments[2]);
        CheatCmdRef.stUnlockLevel = new CSDT_CHEAT_UNLOCK_LEVEL();
        storeyc.DiffType = (tag != ELevelTypeTag.普通) ? RES_LEVEL_DIFFICULTY_TYPE.RES_LEVEL_DIFFICULTY_TYPE_NIGHTMARE : RES_LEVEL_DIFFICULTY_TYPE.RES_LEVEL_DIFFICULTY_TYPE_NORMAL;
        ResLevelCfgInfo info = GameDataMgr.levelDatabin.FindIf(new Func<ResLevelCfgInfo, bool>(storeyc, (IntPtr) this.<>m__4));
        if (info != null)
        {
            CheatCmdRef.stUnlockLevel.iLevelID = info.iCfgID;
            return CheatCommandBase.Done;
        }
        return string.Format("未找到 {2} {0}-{1}对应地图配置", storeyc.Chapter, storeyc.No, tag.ToString());
    }

    [CompilerGenerated]
    private sealed class <Execute>c__AnonStorey3C
    {
        internal int Chapter;
        internal RES_LEVEL_DIFFICULTY_TYPE DiffType;
        internal int No;

        internal bool <>m__4(ResLevelCfgInfo x)
        {
            return (((x.iChapterId == this.Chapter) && (x.bLevelNo == this.No)) && (x.bLevelDifficulty == ((byte) this.DiffType)));
        }
    }
}

