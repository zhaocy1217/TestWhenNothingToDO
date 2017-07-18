namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Framework;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AnimPlayComponent : LogicComponent
    {
        [CompilerGenerated]
        private static Comparison<PlayAnimParam> <>f__am$cache5;
        private List<PlayAnimParam> anims = new List<PlayAnimParam>(3);
        public bool bPausePlay;
        private List<Assets.Scripts.GameLogic.ChangeAnimParam> changeList = new List<Assets.Scripts.GameLogic.ChangeAnimParam>(2);
        private string curAnimName;
        private ulong curAnimPlayFrameTick;

        public override void Born(ActorRoot owner)
        {
            base.Born(owner);
            int num = base.gameObject.get_transform().get_childCount();
            for (int i = 0; i < num; i++)
            {
                GameObject inActorMesh = base.gameObject.get_transform().GetChild(i).get_gameObject();
                if (inActorMesh.GetComponent<Animation>() != null)
                {
                    base.actor.SetActorMesh(inActorMesh);
                    base.actor.RecordOriginalActorMesh();
                    break;
                }
            }
        }

        private void ChangeAnimName(ref PlayAnimParam param)
        {
            for (int i = 0; i < this.changeList.Count; i++)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param2 = this.changeList[i];
                if ((param2.originalAnimName == param.animName) && (base.actor.ActorMeshAnimation.GetClip(param2.changedAnimName) != null))
                {
                    param.animName = param2.changedAnimName;
                    return;
                }
            }
        }

        public void ChangeAnimParam(string _oldAnimName, string _newAnimName)
        {
            Assets.Scripts.GameLogic.ChangeAnimParam item = new Assets.Scripts.GameLogic.ChangeAnimParam();
            item.originalAnimName = _oldAnimName;
            item.changedAnimName = _newAnimName;
            this.changeList.Add(item);
            this.ChangeCurAnimParam(item, false);
        }

        private void ChangeCurAnimParam(Assets.Scripts.GameLogic.ChangeAnimParam _param, bool bRecover)
        {
            string str2 = !bRecover ? _param.originalAnimName : _param.changedAnimName;
            string str3 = !bRecover ? _param.changedAnimName : _param.originalAnimName;
            for (int i = 0; i < this.anims.Count; i++)
            {
                PlayAnimParam param = this.anims[i];
                if (param.animName == str2)
                {
                    string animName = param.animName;
                    param.animName = str3;
                    if (animName == this.curAnimName)
                    {
                        this.Play(param);
                    }
                }
            }
        }

        private void ClearVariables()
        {
            this.anims.Clear();
            this.changeList.Clear();
            this.curAnimName = null;
            this.curAnimPlayFrameTick = 0L;
            this.bPausePlay = false;
        }

        public override void Deactive()
        {
            this.ClearVariables();
            base.Deactive();
        }

        private void DoPlay(PlayAnimParam param)
        {
            if (!this.bPausePlay)
            {
                if (param.blendTime > 0f)
                {
                    if (!param.loop)
                    {
                        AnimationState state = base.actor.ActorMeshAnimation.CrossFadeQueued(param.animName, param.blendTime, 2);
                        if (state != null)
                        {
                            state.set_speed(param.speed);
                            state.set_wrapMode(!param.loop ? ((WrapMode) 8) : ((WrapMode) 2));
                        }
                    }
                    else
                    {
                        base.actor.ActorMeshAnimation.CrossFade(param.animName, param.blendTime);
                    }
                }
                else
                {
                    base.actor.ActorMeshAnimation.Stop();
                    base.actor.ActorMeshAnimation.Play(param.animName);
                }
                this.curAnimName = param.animName;
                this.curAnimPlayFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
                AnimationState state2 = base.actor.ActorMeshAnimation.get_Item(param.animName);
                if (state2 != null)
                {
                    state2.set_wrapMode(!param.loop ? ((WrapMode) 8) : ((WrapMode) 2));
                }
            }
        }

        private string GetChangeAnimName(string changeName)
        {
            for (int i = 0; i < this.changeList.Count; i++)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param = this.changeList[i];
                if ((param.originalAnimName == changeName) && (base.actor.ActorMeshAnimation.GetClip(param.changedAnimName) != null))
                {
                    return param.changedAnimName;
                }
            }
            return changeName;
        }

        public string GetCurAnimName()
        {
            return this.curAnimName;
        }

        public override void OnUse()
        {
            base.OnUse();
            this.ClearVariables();
        }

        public void Play(PlayAnimParam param)
        {
            if (((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null)) && (base.actor.ActorMeshAnimation.GetClip(param.animName) != null))
            {
                if (this.changeList.Count > 0)
                {
                    this.ChangeAnimName(ref param);
                }
                if (param.cancelAll)
                {
                    this.anims.Clear();
                }
                if (param.cancelCurrent && (this.anims.Count > 0))
                {
                    this.anims.RemoveAt(this.anims.Count - 1);
                }
                for (int i = 0; i < this.anims.Count; i++)
                {
                    PlayAnimParam param2 = this.anims[i];
                    if (param2.layer == param.layer)
                    {
                        this.anims.RemoveAt(i);
                        i--;
                    }
                }
                this.anims.Add(param);
                bool flag = true;
                if (this.anims.Count > 1)
                {
                    if (<>f__am$cache5 == null)
                    {
                        <>f__am$cache5 = delegate (PlayAnimParam a, PlayAnimParam b) {
                            if (a.layer == b.layer)
                            {
                                return 0;
                            }
                            if (a.layer < b.layer)
                            {
                                return -1;
                            }
                            return 1;
                        };
                    }
                    this.anims.Sort(<>f__am$cache5);
                    PlayAnimParam param3 = this.anims[this.anims.Count - 1];
                    flag = param3.animName == param.animName;
                }
                if (flag)
                {
                    this.DoPlay(param);
                }
            }
        }

        public void RecoverAnimParam(string _changeAnimName)
        {
            int index = -1;
            for (int i = 0; i < this.changeList.Count; i++)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param2 = this.changeList[i];
                if (param2.originalAnimName == _changeAnimName)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                Assets.Scripts.GameLogic.ChangeAnimParam param = this.changeList[index];
                this.changeList.RemoveAt(index);
                this.ChangeCurAnimParam(param, true);
            }
        }

        public void SetAnimPlaySpeed(string clipName, float speed)
        {
            if ((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null))
            {
                AnimationState state = base.actor.ActorMeshAnimation.get_Item(clipName);
                if ((state != null) && (state.get_speed() != 0f))
                {
                    state.set_speed(speed);
                }
            }
        }

        public void Stop(string origAnimName, bool bFlag = false)
        {
            if ((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null))
            {
                string changeAnimName = this.GetChangeAnimName(origAnimName);
                if (base.actor.ActorMeshAnimation.GetClip(changeAnimName) != null)
                {
                    bool flag = false;
                    int count = this.anims.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        PlayAnimParam param = this.anims[i];
                        if (param.animName == changeAnimName)
                        {
                            flag = i == (count - 1);
                            this.anims.RemoveAt(i);
                        }
                    }
                    count = this.anims.Count;
                    if (flag)
                    {
                        if (!bFlag)
                        {
                            if (count <= 0)
                            {
                                return;
                            }
                            PlayAnimParam param2 = this.anims[count - 1];
                            if (!param2.forceOutOfStack)
                            {
                                return;
                            }
                        }
                        if (count > 0)
                        {
                            this.DoPlay(this.anims[count - 1]);
                        }
                        else
                        {
                            base.actor.ActorMeshAnimation.get_Item(changeAnimName).set_enabled(false);
                        }
                    }
                }
            }
        }

        public void UpdateCurAnimState()
        {
            if (((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null)) && ((this.curAnimName != null) && (base.actor.ActorMeshAnimation.GetClip(this.curAnimName) != null)))
            {
                Animation actorMeshAnimation = base.actor.ActorMeshAnimation;
                FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
                AnimationState state = actorMeshAnimation.get_Item(this.curAnimName);
                float num = (float) (((double) (instance.LogicFrameTick - this.curAnimPlayFrameTick)) / 1000.0);
                if (state.get_wrapMode() == 2)
                {
                    if (state.get_length() == 0f)
                    {
                        num = 0f;
                    }
                    else
                    {
                        int num2 = (int) (num / state.get_length());
                        num -= num2 * state.get_length();
                    }
                    actorMeshAnimation.Play(this.curAnimName);
                    state.set_time(num);
                }
                else
                {
                    if (num >= state.get_length())
                    {
                        num = state.get_length();
                    }
                    state.set_time(num);
                }
            }
        }

        public override void UpdateLogic(int delta)
        {
            if (((base.actor.ActorMesh != null) && (base.actor.ActorMeshAnimation != null)) && !base.actor.ActorMeshAnimation.get_isPlaying())
            {
                int count = this.anims.Count;
                if (count > 0)
                {
                    this.anims.RemoveAt(count - 1);
                    count--;
                    if (count > 0)
                    {
                        this.DoPlay(this.anims[count - 1]);
                    }
                }
            }
        }

        public void UpdatePlay()
        {
            if (this.anims.Count > 0)
            {
                this.DoPlay(this.anims[this.anims.Count - 1]);
            }
        }
    }
}

