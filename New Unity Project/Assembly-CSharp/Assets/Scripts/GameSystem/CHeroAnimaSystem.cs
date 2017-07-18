namespace Assets.Scripts.GameSystem
{
    using Assets.Scripts.Framework;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.UI;
    using ResData;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CHeroAnimaSystem : Singleton<CHeroAnimaSystem>
    {
        private GameObject m_3DModel;
        private List<string> m_animatList = new List<string>();
        private ListView<AnimaSoundElement> m_animatSoundList = new ListView<AnimaSoundElement>();
        private List<string> m_clikAnimaList = new List<string>();
        private ulong m_tickAnimat;
        private static string[] s_noClikAnima = new string[] { "Come", "Cheer" };

        public override void Init()
        {
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ModelDrag, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_DragModel));
            Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ModelClick, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ClickModel));
        }

        public void InitAnimatList()
        {
            this.m_animatList.Clear();
            this.m_clikAnimaList.Clear();
            if (this.m_3DModel != null)
            {
                Animation component = this.m_3DModel.GetComponent<Animation>();
                if ((component != null) && (component.get_clip() != null))
                {
                    IEnumerator enumerator = component.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        AnimationState current = (AnimationState) enumerator.Current;
                        if ((current.get_clip() != null) && !string.IsNullOrEmpty(current.get_clip().get_name()))
                        {
                            this.m_animatList.Add(current.get_clip().get_name());
                            if ((current.get_clip().get_name() != component.get_clip().get_name()) && !this.IsNoClickAnima(current.get_clip().get_name()))
                            {
                                this.m_clikAnimaList.Add(current.get_clip().get_name());
                            }
                        }
                    }
                    if (component.get_cullingType() != null)
                    {
                        component.set_cullingType(0);
                    }
                }
            }
        }

        public void InitAnimatSoundList(uint heroId, uint skinId)
        {
            this.m_animatSoundList.Clear();
            ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
            if (dataByKey != null)
            {
                CActorInfo content = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataByKey.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
                if (content != null)
                {
                    Singleton<CSoundManager>.GetInstance().LoadSkinSoundBank(heroId, skinId, this.m_3DModel, true);
                    for (int i = 0; i < content.AnimaSound.Length; i++)
                    {
                        this.m_animatSoundList.Add(content.AnimaSound[i]);
                    }
                }
            }
        }

        protected bool IsNoClickAnima(string aniName)
        {
            for (int i = 0; i < s_noClikAnima.Length; i++)
            {
                if (s_noClikAnima[i] == aniName)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnHeroInfo_ClickModel(CUIEvent uiEvent)
        {
            if (this.m_3DModel != null)
            {
                Animation component = this.m_3DModel.GetComponent<Animation>();
                if ((((component != null) && (component.get_clip() != null)) && (component.IsPlaying(component.get_clip().get_name()) || ((Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_tickAnimat) >= 0x7d0L))) && (((component != null) && (this.m_clikAnimaList != null)) && (this.m_clikAnimaList.Count > 0)))
                {
                    int num = Random.Range(0, this.m_clikAnimaList.Count);
                    component.CrossFade(this.m_clikAnimaList[num]);
                    component.CrossFadeQueued(component.get_clip().get_name(), 0.3f);
                    this.PlayAnimaSound(this.m_clikAnimaList[num]);
                    this.m_tickAnimat = Singleton<FrameSynchr>.instance.LogicFrameTick;
                }
            }
        }

        public void OnHeroInfo_DragModel(CUIEvent uiEvent)
        {
            if (this.m_3DModel != null)
            {
                this.m_3DModel.get_transform().get_parent().Rotate(0f, -uiEvent.m_pointerEventData.get_delta().x, 0f, 1);
            }
        }

        public void OnModePlayAnima(string animaName)
        {
            if (this.m_3DModel != null)
            {
                Animation component = this.m_3DModel.GetComponent<Animation>();
                if (((component != null) && (component.get_clip() != null)) && this.m_animatList.Contains(animaName))
                {
                    component.Play(animaName);
                    component.CrossFadeQueued(component.get_clip().get_name(), 0.3f);
                    this.PlayAnimaSound(animaName);
                }
            }
        }

        protected void PlayAnimaSound(string aniName)
        {
            for (int i = 0; i < this.m_animatSoundList.Count; i++)
            {
                if ((this.m_animatSoundList[i].AnimaName == aniName) && !string.IsNullOrEmpty(this.m_animatSoundList[i].SoundName))
                {
                    Singleton<CSoundManager>.GetInstance().PostEvent(this.m_animatSoundList[i].SoundName, this.m_3DModel);
                }
            }
        }

        public void Set3DModel(GameObject model)
        {
            this.m_3DModel = model;
            if ((this.m_3DModel != null) && (this.m_3DModel.GetComponent<AkAudioListener>() == null))
            {
                this.m_3DModel.AddComponent<AkAudioListener>();
            }
        }

        public override void UnInit()
        {
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_ModelDrag, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_DragModel));
            Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_ModelClick, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_ClickModel));
        }
    }
}

