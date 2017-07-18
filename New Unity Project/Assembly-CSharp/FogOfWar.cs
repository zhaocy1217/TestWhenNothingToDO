using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

public class FogOfWar
{
    private static byte[] _bitmapData = null;
    public static int _BitmapHeight = 0x200;
    private static Texture2D _bitmapTexture = null;
    public static int _BitmapWidth = 0x200;
    private static bool _enable = false;
    private static long commitFrame = 0L;
    public static readonly byte FOG_DELAY_FRAME = 90;
    private static Material[] fowMats = new Material[3];
    private static RenderTexture[] fowTextures = new RenderTexture[3];
    public static bool ms_bDrawDebugLineTexts = true;
    public static uint RenderFrameNum = 0;

    public static void BeginLevel()
    {
        DisableShaderFogFunction();
        if (enable)
        {
            GC.Collect();
            GameObject obj2 = GameObject.Find("Design/Field");
            if ((obj2 != null) && obj2.get_activeInHierarchy())
            {
                FieldObj component = obj2.GetComponent<FieldObj>();
                if (component != null)
                {
                    if (!Singleton<WatchController>.instance.IsWatching)
                    {
                        EnableShaderFogFunction();
                    }
                    component.InitField();
                    int gridUnits = 0;
                    component.UnrealToGridX(Horizon.QueryMainActorFakeSightRadius(), out gridUnits);
                    Singleton<GameFowManager>.instance.InitSurface(true, component, gridUnits);
                    if (Singleton<GameFowManager>.instance.LoadPrecomputeData())
                    {
                        Reset(component.FieldX, component.FieldY, component.NumX, component.NumY, (int) GameDataMgr.globalInfoDatabin.GetDataByKey((uint) 0x38).dwConfValue);
                        ClearAllFog(true);
                        float num2 = Mathf.Max(((float) component.FieldX) / 1000f, 1f);
                        float num3 = Mathf.Max(((float) component.FieldY) / 1000f, 1f);
                        Shader.SetGlobalVector("_InvSceneSize", new Vector4(1f / num2, 1f / num3, num2, num3));
                    }
                }
            }
        }
    }

    private static void Clear()
    {
        Shader.SetGlobalTexture("_FogOfWar", null);
        _bitmapTexture = null;
        for (int i = 0; i < fowTextures.Length; i++)
        {
            if (fowTextures[i] != null)
            {
                fowTextures[i].Release();
            }
        }
    }

    private static void ClearAllFog(bool bCommit)
    {
        if (_bitmapData != null)
        {
            for (int i = 0; i < _bitmapData.Length; i++)
            {
                _bitmapData[i] = 0xff;
            }
            if (bCommit)
            {
                CommitToMaterials();
            }
        }
    }

    public static void CommitToMaterials()
    {
        if (CheatCommandReplayEntry.commitFOWMaterial && (_bitmapData != null))
        {
            _bitmapTexture.LoadRawTextureData(_bitmapData);
            _bitmapTexture.Apply();
            RenderTexture texture = fowTextures[0];
            fowTextures[0] = fowTextures[1];
            fowTextures[1] = texture;
            fowTextures[0].DiscardContents();
            Graphics.Blit(_bitmapTexture, fowTextures[0]);
            SetBlurVectors(1f / ((float) fowTextures[0].get_width()), 0f);
            fowTextures[2].DiscardContents();
            Graphics.Blit(fowTextures[0], fowTextures[2], fowMats[0]);
            SetBlurVectors(0f, 1f / ((float) fowTextures[0].get_height()));
            fowTextures[0].DiscardContents();
            Graphics.Blit(fowTextures[2], fowTextures[0], fowMats[0]);
            RenderTexture.set_active(null);
            commitFrame = Singleton<FrameSynchr>.instance.CurFrameNum;
            UpdateTextures();
        }
    }

    public static void CopyBitmap()
    {
        VInt2 mainActorFakeSightPos = MainActorFakeSightPos;
        FieldObj pFieldObj = Singleton<GameFowManager>.instance.m_pFieldObj;
        FowLos.PreCopyBitmap(mainActorFakeSightPos.x, mainActorFakeSightPos.y, pFieldObj.NumX, pFieldObj.NumY, Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
        _bitmapData = Singleton<GameFowManager>.instance.GetCommitPixels();
    }

    private static void CreateMat(ref Material mat, string name)
    {
        if (mat == null)
        {
            Shader shader = Shader.Find(name);
            if (shader == null)
            {
            }
            mat = new Material(shader);
        }
    }

    private static void DisableShaderFogFunction()
    {
        Shader.DisableKeyword("_FOG_OF_WAR_ON");
        Shader.DisableKeyword("_FOG_OF_WAR_ON_LOW");
    }

    private static void EnableShaderFogFunction()
    {
        if (!GameSettings.IsHighQuality)
        {
            Shader.DisableKeyword("_FOG_OF_WAR_ON");
            Shader.EnableKeyword("_FOG_OF_WAR_ON_LOW");
        }
        else
        {
            Shader.DisableKeyword("_FOG_OF_WAR_ON_LOW");
            Shader.EnableKeyword("_FOG_OF_WAR_ON");
        }
    }

    public static void EndLevel()
    {
        DisableShaderFogFunction();
        enable = false;
        Singleton<GameFowManager>.instance.UninitSurface();
        Clear();
        _bitmapData = null;
        RenderFrameNum = 0;
    }

    private static void LightFowTex(int x, int y, int radius)
    {
        Vector4 vector = new Vector4();
        vector.x = ((float) radius) / ((float) _bitmapTexture.get_width());
        vector.y = ((float) radius) / ((float) _bitmapTexture.get_height());
        vector.z = ((x * 2f) / ((float) _bitmapTexture.get_width())) - 1f;
        vector.w = ((y * 2f) / ((float) _bitmapTexture.get_height())) - 1f;
        Vector4 vector2 = new Vector4();
        vector2.x = _bitmapTexture.get_width();
        vector2.y = _bitmapTexture.get_height();
        vector2.z = radius * radius;
        Material material = fowMats[2];
        material.SetVector("_Transform", vector);
        material.SetVector("_Transform2", vector2);
        Graphics.Blit(_bitmapTexture, fowTextures[0], material);
    }

    private static int Power2(int value)
    {
        return (value * value);
    }

    public static void PreBeginLevel()
    {
        GameObject obj2 = GameObject.Find("Design/Field");
        if ((obj2 != null) && obj2.get_activeInHierarchy())
        {
            FieldObj component = obj2.GetComponent<FieldObj>();
            if ((((component != null) && component.bSynced) && ((component.fowOfflineData != null) && (component.fowOfflineData.Length > 0))) && Singleton<BattleLogic>.instance.GetCurLvelContext().m_bEnableFow)
            {
                enable = true;
            }
        }
    }

    public static void PrepareData()
    {
        BattleLogic instance = Singleton<BattleLogic>.instance;
        if (enable && instance.isFighting)
        {
            SLevelContext curLvelContext = instance.GetCurLvelContext();
            if ((curLvelContext != null) && (curLvelContext.m_horizonEnableMethod == Horizon.EnableMethod.EnableAll))
            {
                GameFowManager manager = Singleton<GameFowManager>.instance;
                GameFowCollector collector = manager.m_collector;
                collector.UpdateFowVisibility(false);
                collector.CollectExplorer(false);
                if ((Singleton<FrameSynchr>.instance.CurFrameNum % manager.GPUInterpolateFrameInterval) == 0)
                {
                    CommitToMaterials();
                }
            }
        }
    }

    private static void Reset(int inMapWidth, int inMapHeight, int inBitmapWidth, int inBitmapHeight, int sight)
    {
        _BitmapWidth = inBitmapWidth;
        _BitmapHeight = inBitmapHeight;
        if (((inBitmapWidth != 0) && (inBitmapHeight != 0)) && (sight != 0))
        {
            Clear();
            if (_bitmapTexture != null)
            {
                _bitmapTexture.Resize(_BitmapWidth, _BitmapHeight, 1, false);
            }
            else
            {
                _bitmapTexture = new Texture2D(_BitmapWidth, _BitmapHeight, 1, false);
                _bitmapTexture.set_wrapMode(1);
            }
            for (int i = 0; i < fowTextures.Length; i++)
            {
                if (fowTextures[i] != null)
                {
                    fowTextures[i].Release();
                }
                int x = _BitmapWidth * 2;
                int num3 = _BitmapHeight * 2;
                if (SystemInfo.get_npotSupport() == null)
                {
                    x = IntMath.CeilPowerOfTwo(x);
                    num3 = IntMath.CeilPowerOfTwo(num3);
                }
                fowTextures[i] = new RenderTexture(x, num3, 0, 7);
                fowTextures[i].set_wrapMode(1);
            }
            CreateMat(ref fowMats[0], "SGame_Post/FowBlur");
            CreateMat(ref fowMats[1], "SGame_Post/FowInterpolate");
            CreateMat(ref fowMats[2], "SGame_Post/FowLight");
            if ((inMapWidth != 0) && (inMapHeight != 0))
            {
            }
        }
    }

    public static void Run()
    {
        if (enable && Singleton<BattleLogic>.instance.isFighting)
        {
            GameFowManager instance = Singleton<GameFowManager>.instance;
            instance.UpdateComputing();
            if ((Singleton<FrameSynchr>.instance.CurFrameNum % instance.GPUInterpolateFrameInterval) == 0)
            {
                CopyBitmap();
            }
        }
    }

    private static void SetBlurVectors(float sizeX, float sizeY)
    {
        Vector4 vector = new Vector4(sizeX, sizeY, -sizeX, -sizeY);
        fowMats[0].SetVector("_BlurDir1", (Vector4) (vector * 0.5f));
        fowMats[0].SetVector("_BlurDir2", (Vector4) (vector * 1.5f));
    }

    public static void SetFlip(bool flip)
    {
        if (flip)
        {
            Shader.EnableKeyword("_FOG_OF_WAR_FLIP_ON");
        }
        else
        {
            Shader.EnableKeyword("_FOG_OF_WAR_FLIP_OFF");
        }
    }

    public static void UpdateMain()
    {
        BattleLogic instance = Singleton<BattleLogic>.instance;
        if (enable && instance.isFighting)
        {
            SLevelContext curLvelContext = instance.GetCurLvelContext();
            if ((curLvelContext != null) && (curLvelContext.m_horizonEnableMethod == Horizon.EnableMethod.EnableAll))
            {
                GameFowManager manager = Singleton<GameFowManager>.instance;
                if ((Singleton<FrameSynchr>.instance.CurFrameNum % manager.GPUInterpolateFrameInterval) == 0)
                {
                    CopyBitmap();
                    CommitToMaterials();
                }
                GameFowCollector collector = manager.m_collector;
                collector.UpdateFowVisibility(false);
                collector.CollectExplorer(false);
                manager.UpdateComputing();
            }
        }
    }

    public static void UpdateTextures()
    {
        if (enable)
        {
            FrameSynchr instance = Singleton<FrameSynchr>.instance;
            float num = (instance.CurFrameNum - commitFrame) * Singleton<GameFowManager>.instance.GPUInterpolateReciprocal;
            if (instance.bActive)
            {
                uint num2 = (uint) ((Time.get_realtimeSinceStartup() - instance.startFrameTime) * 1000f);
                num2 *= instance.FrameSpeed;
                uint num3 = instance.CurFrameNum * instance.FrameDelta;
                int num4 = (int) (num2 - num3);
                num4 -= instance.nJitterDelay;
                num4 = Mathf.Clamp(num4, 0, (int) instance.FrameDelta);
                num += (((float) num4) / ((float) instance.FrameDelta)) * Singleton<GameFowManager>.instance.GPUInterpolateReciprocal;
            }
            num = Mathf.Clamp01(num);
            fowMats[1].SetTexture("_FowTex0", fowTextures[1]);
            fowMats[1].SetTexture("_FowTex1", fowTextures[0]);
            fowMats[1].SetFloat("_Factor", num);
            fowTextures[2].DiscardContents();
            Graphics.Blit(null, fowTextures[2], fowMats[1]);
            RenderTexture.set_active(null);
            fowMats[1].SetTexture("_FowTex0", null);
            fowMats[1].SetTexture("_FowTex1", null);
            Shader.SetGlobalTexture("_FogOfWar", fowTextures[2]);
        }
    }

    public static bool enable
    {
        get
        {
            return _enable;
        }
        set
        {
            if (_enable != value)
            {
                _enable = value;
                if (!_enable)
                {
                    ClearAllFog(false);
                }
            }
        }
    }

    public static VInt2 MainActorFakeSightPos
    {
        get
        {
            Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
            if ((hostPlayer != null) && (hostPlayer.Captain != 0))
            {
                VInt3 location = hostPlayer.Captain.handle.location;
                location = new VInt3(location.x, location.z, 0);
                VInt2 zero = VInt2.zero;
                if (Singleton<GameFowManager>.instance.WorldPosToGrid(location, out zero.x, out zero.y))
                {
                    return zero;
                }
            }
            return VInt2.zero;
        }
    }
}

