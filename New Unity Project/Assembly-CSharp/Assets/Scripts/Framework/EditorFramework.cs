namespace Assets.Scripts.Framework
{
    using AGE;
    using Assets.Scripts.GameLogic;
    using Assets.Scripts.GameSystem;
    using CSProtocol;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class EditorFramework : GameFramework
    {
        protected override void Init()
        {
            base.Init();
            base.StartPrepareBaseSystem(new GameFramework.DelegateOnBaseSystemPrepareComplete(this.OnPrepareBaseSystemComplete));
        }

        private void OnPrepareBaseSystemComplete()
        {
            base.StartCoroutine(this.StartPrepareGameSystem());
        }

        public override void Start()
        {
            Singleton<ApolloHelper>.GetInstance();
            Singleton<GameStateCtrl>.GetInstance().Initialize();
        }

        [DebuggerHidden]
        private IEnumerator StartPrepareGameSystem()
        {
            <StartPrepareGameSystem>c__Iterator3 iterator = new <StartPrepareGameSystem>c__Iterator3();
            iterator.<>f__this = this;
            return iterator;
        }

        [CompilerGenerated]
        private sealed class <StartPrepareGameSystem>c__Iterator3 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal EditorFramework <>f__this;
            internal COMDT_CLIENT_BITS <pp2>__0;
            internal SCPKG_STARTSINGLEGAMERSP <simuData>__1;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = this.<>f__this.StartCoroutine(this.<>f__this.PrepareGameSystem());
                        this.$PC = 1;
                        return true;

                    case 1:
                        ActionManager.Instance.frameMode = true;
                        Singleton<CRoleInfoManager>.GetInstance().SetMaterUUID(0L);
                        Singleton<CRoleInfoManager>.GetInstance().CreateRoleInfo(enROLEINFO_TYPE.PLAYER, 0L, 0x3e9);
                        this.<pp2>__0 = new COMDT_CLIENT_BITS();
                        this.<pp2>__0.BitsDetail[0] = ulong.MaxValue;
                        this.<pp2>__0.BitsDetail[1] = ulong.MaxValue;
                        Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitNewbieAchieveBits(this.<pp2>__0);
                        CSkinInfo.InitHeroSkinDicData();
                        this.<simuData>__1 = new SCPKG_STARTSINGLEGAMERSP();
                        this.<simuData>__1.stDetail = new CSDT_SINGLEGAME_DETAIL();
                        this.<simuData>__1.stDetail.stSingleGameSucc = new CSDT_BATTLE_PLAYER_BRIEF();
                        this.<simuData>__1.stDetail.stSingleGameSucc.bNum = 1;
                        this.<simuData>__1.stDetail.stSingleGameSucc.astFighter = new COMDT_PLAYERINFO[] { new COMDT_PLAYERINFO() };
                        this.<simuData>__1.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = (uint) PlayerPrefs.GetInt("PrevewHeroID");
                        this.<simuData>__1.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID = (ushort) PlayerPrefs.GetInt("PrevewSkinID");
                        Singleton<GameBuilder>.instance.StartGame(new TestGameContext(ref this.<simuData>__1));
                        Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

