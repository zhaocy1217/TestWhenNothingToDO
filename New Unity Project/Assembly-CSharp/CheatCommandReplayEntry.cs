using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

[CheatCommandEntry("性能")]
internal class CheatCommandReplayEntry
{
    public static bool commitFOWMaterial = true;
    public static bool heroPerformanceTest;

    [CheatCommandEntryMethod("Battle Form模式", true, false)]
    public static string BattleFormCanvasMode()
    {
        Canvas component = Singleton<CBattleSystem>.GetInstance().FormScript.get_gameObject().GetComponent<Canvas>();
        if (component.get_renderMode() == 1)
        {
            component.set_renderMode(2);
        }
        else if (component.get_renderMode() == 2)
        {
            component.set_renderMode(1);
        }
        return component.get_renderMode().ToString();
    }

    [CheatCommandEntryMethod(" 关闭战斗UI", true, false)]
    public static string CloseFormBattle()
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm);
        if (form != null)
        {
            form.get_gameObject().CustomSetActive(false);
            return "已关闭";
        }
        return "不在战斗中";
    }

    [CheatCommandEntryMethod("迷雾材质提交开关", true, false)]
    public static string CommitFOWMaterialSwitch()
    {
        commitFOWMaterial = !commitFOWMaterial;
        return commitFOWMaterial.ToString();
    }

    [CheatCommandEntryMethod(" 锁帧模式", true, false)]
    public static string LockFPS()
    {
        GameFramework instance = MonoSingleton<GameFramework>.GetInstance();
        instance.LockFPS_SGame = !instance.LockFPS_SGame;
        return (!instance.LockFPS_SGame ? "UNITY" : "SGAME");
    }

    [CheatCommandEntryMethod("小地图开关", true, false)]
    public static string MapPanelSwitch()
    {
        GameObject obj2 = Singleton<CBattleSystem>.GetInstance().FormScript.get_transform().Find("MapPanel").get_gameObject();
        obj2.SetActive(!obj2.get_activeSelf());
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("PROFILE!", true, false)]
    public static string Profile()
    {
        MonoSingleton<ConsoleWindow>.instance.isVisible = false;
        MonoSingleton<SProfiler>.GetInstance().ToggleVisible();
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("PROFILE 10000!", true, false)]
    public static string Profile10000()
    {
        MonoSingleton<ConsoleWindow>.instance.isVisible = false;
        MonoSingleton<SProfiler>.GetInstance().StartProfileNFrames(0x2710, 0);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("PROFILE 5000!", true, false)]
    public static string Profile5000()
    {
        MonoSingleton<ConsoleWindow>.instance.isVisible = false;
        MonoSingleton<SProfiler>.GetInstance().StartProfileNFrames(0x1388, 0);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("PROFILE 300->5000!", true, false)]
    public static string Profile5000Auto()
    {
        FrameCommand<SwitchActorAutoAICommand> command = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
        command.cmdData.IsAutoAI = 1;
        command.Send();
        MonoSingleton<ConsoleWindow>.instance.isVisible = false;
        MonoSingleton<SProfiler>.GetInstance().StartProfileNFrames(0x1388, 300);
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod(" 战斗UI恢复显示和绘制", true, false)]
    public static string ShowFormBattle()
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm);
        if (form != null)
        {
            form.get_gameObject().CustomSetActive(true);
            form.Appear(enFormHideFlag.HideByCustom, true);
            return "已恢复";
        }
        return "不在战斗中";
    }

    [CheatCommandEntryMethod("单英雄测试 关", true, false)]
    public static string SingleHeroTestOff()
    {
        heroPerformanceTest = false;
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod("单英雄测试 开", true, false)]
    public static string SingleHeroTestOn()
    {
        heroPerformanceTest = true;
        return CheatCommandBase.Done;
    }

    [CheatCommandEntryMethod(" 战斗UI停止绘制", true, false)]
    public static string StopFormBattleDraw()
    {
        CUIFormScript form = Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm);
        if (form != null)
        {
            form.get_gameObject().CustomSetActive(true);
            form.Hide(enFormHideFlag.HideByCustom, true);
            return "已停止绘制";
        }
        return "不在战斗中";
    }
}

