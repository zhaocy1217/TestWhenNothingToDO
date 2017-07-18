using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class SMaterialEffect_Base : PooledClassObject
{
    public MaterialHurtEffect owner;
    public int playingId;
    public static int s_playingId;
    public string shaderKeyword;
    public int type;

    protected SMaterialEffect_Base()
    {
    }

    public void AllocId()
    {
        this.playingId = ++s_playingId;
    }

    public static void enableMatsKeyword(ListView<Material> mats, string keywords, bool enable)
    {
        if (mats != null)
        {
            if (enable)
            {
                for (int i = 0; i < mats.Count; i++)
                {
                    mats[i].EnableKeyword(keywords);
                }
            }
            else
            {
                for (int j = 0; j < mats.Count; j++)
                {
                    mats[j].DisableKeyword(keywords);
                }
            }
        }
    }

    public virtual void OnMeshChanged(ListView<Material> oldMats, ListView<Material> newMats)
    {
        enableMatsKeyword(oldMats, this.shaderKeyword, false);
        enableMatsKeyword(newMats, this.shaderKeyword, true);
    }

    public virtual void Play()
    {
        enableMatsKeyword(this.owner.mats, this.shaderKeyword, true);
    }

    public virtual void Stop()
    {
        enableMatsKeyword(this.owner.mats, this.shaderKeyword, false);
    }

    public abstract bool Update();
    public static bool UpdateFadeState(out float factor, ref FadeState fadeState, ref STimer fadeTime, float fadeInterval, bool forceUpdate = false)
    {
        bool flag = false;
        factor = 1f;
        if (fadeState == FadeState.FadeIn)
        {
            float num = fadeTime.Update();
            if (num >= fadeInterval)
            {
                factor = 1f;
                fadeState = FadeState.Normal;
            }
            else
            {
                factor = num / fadeInterval;
            }
            flag = true;
        }
        else if (fadeState == FadeState.FadeOut)
        {
            float num2 = fadeTime.Update();
            if (num2 >= fadeInterval)
            {
                factor = 0f;
                fadeState = FadeState.Stopped;
            }
            else
            {
                factor = 1f - (num2 / fadeInterval);
            }
            flag = true;
        }
        if (forceUpdate)
        {
            flag = true;
        }
        return flag;
    }

    public enum FadeState
    {
        FadeIn,
        Normal,
        FadeOut,
        Stopped
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STimer
    {
        public long startFrameTick;
        public float curTime;
        public void Start()
        {
            this.curTime = 0f;
            this.startFrameTick = (long) Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
        }

        public float Update()
        {
            FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
            if (instance.bActive)
            {
                long num3 = ((long) instance.LogicFrameTick) - this.startFrameTick;
                return (num3 * 0.001f);
            }
            this.curTime += Time.get_deltaTime();
            return this.curTime;
        }
    }
}

