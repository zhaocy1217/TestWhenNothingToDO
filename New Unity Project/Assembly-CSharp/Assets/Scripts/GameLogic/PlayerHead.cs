namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using Assets.Scripts.GameLogic.GameKernal;
    using Assets.Scripts.GameSystem;
    using Assets.Scripts.UI;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerHead : MonoBehaviour
    {
        private HeroHeadHud _hudOwner;
        private PoolObjHandle<ActorRoot> _myHero;
        private HeadState _state;
        public Image HeroHeadImg;
        public Image HeroPickImg;
        public Image hpBarImgNormal;
        public Image hpBarImgPicked;
        public GameObject hpRadBar;
        public Text ReviveCdTxt;
        public GameObject SoulBgImgObj;
        public Text SoulLevelTxt;

        private void Awake()
        {
            if (this.ReviveCdTxt != null)
            {
                this.ReviveCdTxt.set_text(string.Empty);
            }
            this.HeroHeadImg = base.get_transform().Find("HeadMask/HeadImg").GetComponent<Image>();
            this.HeroPickImg = base.get_transform().Find("Select").GetComponent<Image>();
            this.SoulLevelTxt = base.get_transform().Find("soulLvlText").GetComponent<Text>();
            if (this.SoulLevelTxt != null)
            {
                this.SoulLevelTxt.set_text("1");
            }
            this.hpRadBar = base.get_transform().Find("progressBar").get_gameObject();
            this.hpBarImgNormal = this.hpRadBar.get_transform().Find("Normal").GetComponent<Image>();
            this.hpBarImgPicked = this.hpRadBar.get_transform().Find("Picked").GetComponent<Image>();
            this.SoulBgImgObj = base.get_transform().Find("soulBgImg").get_gameObject();
            bool bActive = Singleton<BattleLogic>.GetInstance().m_LevelContext.IsSoulGrow();
            this.SoulLevelTxt.get_gameObject().CustomSetActive(bActive);
            this.SoulBgImgObj.get_gameObject().CustomSetActive(bActive);
        }

        public void Init(HeroHeadHud hudOwner, PoolObjHandle<ActorRoot> myHero)
        {
            if (myHero != 0)
            {
                this._hudOwner = hudOwner;
                this._myHero = myHero;
                this.SetState(HeadState.Normal);
                uint configId = (uint) myHero.handle.TheActorMeta.ConfigId;
                this.HeroHeadImg.SetSprite(CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + CSkinInfo.GetHeroSkinPic(configId, 0), Singleton<CBattleSystem>.GetInstance().FightFormScript, true, false, false, false);
                this.OnHeroHpChange(myHero.handle.ValueComponent.actorHp, myHero.handle.ValueComponent.actorHpTotal);
            }
        }

        public void OnHeroHpChange(int curVal, int maxVal)
        {
            if (this.hpRadBar != null)
            {
                float num = ((float) curVal) / ((float) maxVal);
                this.hpBarImgNormal.CustomFillAmount(num);
                this.hpBarImgPicked.CustomFillAmount(num);
            }
        }

        public void OnHeroSoulLvlChange(int level)
        {
            if (this.SoulLevelTxt != null)
            {
                this.SoulLevelTxt.set_text(level.ToString());
            }
        }

        public void OnMyHeroDead()
        {
            if (this._myHero != 0)
            {
                this.SetState(HeadState.ReviveCDing);
                if (this.ReviveCdTxt != null)
                {
                    this.ReviveCdTxt.set_text(string.Format("{0}", Mathf.RoundToInt(this.MyHero.handle.ActorControl.ReviveCooldown * 0.001f)));
                    this.ReviveCdTxt.set_color(Color.get_white());
                    this.ReviveCdTxt.set_fontSize(30);
                }
                if (this.HeroHeadImg != null)
                {
                    this.HeroHeadImg.set_color(new Color(0.3f, 0.3f, 0.3f));
                }
                if (!Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeBurning() && !Singleton<BattleLogic>.instance.GetCurLvelContext().IsGameTypeArena())
                {
                    base.InvokeRepeating("UpdateReviveCd", 1f, 1f);
                }
                else
                {
                    this.ReviveCdTxt.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerHead_Dead"));
                    this.ReviveCdTxt.set_color(Color.get_gray());
                    this.ReviveCdTxt.set_fontSize(14);
                    this.HeroHeadImg.set_color(new Color(0.3f, 0.3f, 0.3f));
                }
            }
        }

        public void OnMyHeroRevive()
        {
            base.CancelInvoke("UpdateReviveCd");
            if (this.ReviveCdTxt != null)
            {
                this.ReviveCdTxt.set_text(string.Empty);
            }
            if (this.HeroHeadImg != null)
            {
                this.HeroHeadImg.set_color(new Color(1f, 1f, 1f));
            }
            this.SetState(HeadState.Normal);
        }

        public void SetPicked(bool isPicked)
        {
            this.hpBarImgNormal.get_gameObject().CustomSetActive(!isPicked);
            this.hpBarImgPicked.get_gameObject().CustomSetActive(isPicked);
            if (this.HeroPickImg != null)
            {
                this.HeroPickImg.get_gameObject().CustomSetActive(isPicked);
            }
            base.GetComponent<RectTransform>().set_localScale(!isPicked ? Vector3.get_one() : this._hudOwner.pickedScale);
        }

        public void SetPrivates(HeadState inHeadState, PoolObjHandle<ActorRoot> inHero, HeroHeadHud inHudOwner)
        {
            this._myHero = inHero;
            this._hudOwner = inHudOwner;
            this.SetState(inHeadState);
        }

        private void SetState(HeadState hs)
        {
            if (hs != this._state)
            {
                this._state = hs;
                base.GetComponent<CUIEventScript>().set_enabled((this._state == HeadState.Normal) || (this._state == HeadState.ReviveReady));
            }
        }

        private void UpdateReviveCd()
        {
            if ((this.MyHero != 0) && (this.MyHero.handle.ActorControl != null))
            {
                int num = Mathf.RoundToInt(this.MyHero.handle.ActorControl.ReviveCooldown * 0.001f);
                if (num >= 0)
                {
                    if (this.ReviveCdTxt != null)
                    {
                        this.ReviveCdTxt.set_text(string.Format("{0}", num));
                    }
                    this.SetState(HeadState.ReviveCDing);
                }
                else if (!Singleton<BattleLogic>.instance.GetCurLvelContext().IsMobaMode())
                {
                    Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this._myHero.handle.TheActorMeta.PlayerId);
                    if (player != null)
                    {
                        HeadState hs = !player.IsMyTeamOutOfBattle() ? HeadState.ReviveForbid : HeadState.ReviveReady;
                        if (hs != this._state)
                        {
                            this.SetState(hs);
                            if (this._state == HeadState.ReviveReady)
                            {
                                this.ReviveCdTxt.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerHead_dianji"));
                                this.ReviveCdTxt.set_color(Color.get_green());
                                this.ReviveCdTxt.set_fontSize(14);
                                this.HeroHeadImg.set_color(new Color(1f, 1f, 1f));
                            }
                            else
                            {
                                this.ReviveCdTxt.set_text(Singleton<CTextManager>.GetInstance().GetText("PlayerHead_tuozhan"));
                                this.ReviveCdTxt.set_color(Color.get_gray());
                                this.ReviveCdTxt.set_fontSize(14);
                                this.HeroHeadImg.set_color(new Color(0.3f, 0.3f, 0.3f));
                            }
                        }
                        Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                        if (((hostPlayer != null) && (this._state == HeadState.ReviveReady)) && hostPlayer.Captain.handle.ActorControl.m_isAutoAI)
                        {
                            this.MyHero.handle.ActorControl.Revive(false);
                        }
                    }
                }
            }
        }

        public HeroHeadHud HudOwner
        {
            get
            {
                return this._hudOwner;
            }
        }

        public PoolObjHandle<ActorRoot> MyHero
        {
            get
            {
                return this._myHero;
            }
        }

        public HeadState state
        {
            get
            {
                return this._state;
            }
        }

        public enum HeadState
        {
            Normal,
            ReviveCDing,
            ReviveReady,
            ReviveForbid
        }
    }
}

