using System;
using System.Runtime.CompilerServices;

[CheatCommand("Fow/SwitchCameraLookDown", "俯视镜头", 0)]
internal class CheatCommandSwitchCameraLookDown : CheatCommandCommon
{
    [CompilerGenerated]
    private static bool <ms_bLookDown>k__BackingField;
    private float m_oldCurZoom;
    private float m_oldDefaultZoom;
    private float m_oldMaxZoom;
    private float m_oldMinZoom;
    private float m_oldRotX;

    protected override string Execute(string[] InArguments)
    {
        if (MonoSingleton<CameraSystem>.instance.MobaCamera.settings.lockTarget != 0)
        {
            if (!ms_bLookDown)
            {
                ms_bLookDown = true;
                this.RecordCameraSettings();
                MonoSingleton<CameraSystem>.instance.MobaCamera.settings.rotation.defualtRotation.x = 90f;
                MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.minZoom = 20f;
                MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.maxZoom = 80f;
                MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.defaultZoom = 35f;
                MonoSingleton<CameraSystem>.instance.SetFocusActorForce(MonoSingleton<CameraSystem>.instance.MobaCamera.settings.lockTarget, MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.defaultZoom);
            }
            else
            {
                ms_bLookDown = false;
                this.RecoverCameraSettings();
                MonoSingleton<CameraSystem>.instance.SetFocusActorForce(MonoSingleton<CameraSystem>.instance.MobaCamera.settings.lockTarget, this.m_oldCurZoom);
            }
        }
        return CheatCommandBase.Done;
    }

    private void RecordCameraSettings()
    {
        this.m_oldRotX = MonoSingleton<CameraSystem>.instance.MobaCamera.settings.rotation.defualtRotation.x;
        this.m_oldMinZoom = MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.minZoom;
        this.m_oldMaxZoom = MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.maxZoom;
        this.m_oldDefaultZoom = MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.defaultZoom;
        this.m_oldCurZoom = MonoSingleton<CameraSystem>.instance.MobaCamera.currentZoomAmount;
    }

    private void RecoverCameraSettings()
    {
        MonoSingleton<CameraSystem>.instance.MobaCamera.settings.rotation.defualtRotation.x = this.m_oldRotX;
        MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.minZoom = this.m_oldMinZoom;
        MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.maxZoom = this.m_oldMaxZoom;
        MonoSingleton<CameraSystem>.instance.MobaCamera.settings.zoom.defaultZoom = this.m_oldDefaultZoom;
    }

    public static bool ms_bLookDown
    {
        [CompilerGenerated]
        get
        {
            return <ms_bLookDown>k__BackingField;
        }
        [CompilerGenerated]
        private set
        {
            <ms_bLookDown>k__BackingField = value;
        }
    }
}

