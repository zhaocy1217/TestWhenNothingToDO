namespace com.tencent.pandora
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public static class DelegateFactory
    {
        private static Dictionary<Type, DelegateValue> dict = new Dictionary<Type, DelegateValue>();

        public static Delegate Action(LuaFunction func)
        {
            <Action>c__AnonStorey85 storey = new <Action>c__AnonStorey85();
            storey.func = func;
            return new System.Action(storey, (IntPtr) this.<>m__9C);
        }

        public static Delegate Action_GameObject(LuaFunction func)
        {
            <Action_GameObject>c__AnonStorey84 storey = new <Action_GameObject>c__AnonStorey84();
            storey.func = func;
            return new Action<GameObject>(storey.<>m__9B);
        }

        public static void Clear()
        {
            dict.Clear();
        }

        [NoToLua]
        public static Delegate CreateDelegate(Type t, LuaFunction func)
        {
            DelegateValue value2 = null;
            if (!dict.TryGetValue(t, out value2))
            {
                Debug.LogError("Delegate " + t.FullName + " not register");
                return null;
            }
            return value2(func);
        }

        [NoToLua]
        public static void Register(IntPtr L)
        {
            dict.Add(typeof(Action<GameObject>), new DelegateValue(DelegateFactory.Action_GameObject));
            dict.Add(typeof(System.Action), new DelegateValue(DelegateFactory.Action));
            dict.Add(typeof(UnityAction), new DelegateValue(DelegateFactory.UnityEngine_Events_UnityAction));
            dict.Add(typeof(MemberFilter), new DelegateValue(DelegateFactory.System_Reflection_MemberFilter));
            dict.Add(typeof(TypeFilter), new DelegateValue(DelegateFactory.System_Reflection_TypeFilter));
        }

        public static Delegate System_Reflection_MemberFilter(LuaFunction func)
        {
            <System_Reflection_MemberFilter>c__AnonStorey87 storey = new <System_Reflection_MemberFilter>c__AnonStorey87();
            storey.func = func;
            return new MemberFilter(storey.<>m__9E);
        }

        public static Delegate System_Reflection_TypeFilter(LuaFunction func)
        {
            <System_Reflection_TypeFilter>c__AnonStorey88 storey = new <System_Reflection_TypeFilter>c__AnonStorey88();
            storey.func = func;
            return new TypeFilter(storey.<>m__9F);
        }

        public static Delegate UnityEngine_Events_UnityAction(LuaFunction func)
        {
            <UnityEngine_Events_UnityAction>c__AnonStorey86 storey = new <UnityEngine_Events_UnityAction>c__AnonStorey86();
            storey.func = func;
            return new UnityAction(storey, (IntPtr) this.<>m__9D);
        }

        [CompilerGenerated]
        private sealed class <Action_GameObject>c__AnonStorey84
        {
            internal LuaFunction func;

            internal void <>m__9B(GameObject param0)
            {
                int oldTop = this.func.BeginPCall();
                LuaScriptMgr.Push(this.func.GetLuaState(), (Object) param0);
                this.func.PCall(oldTop, 1);
                this.func.EndPCall(oldTop);
            }
        }

        [CompilerGenerated]
        private sealed class <Action>c__AnonStorey85
        {
            internal LuaFunction func;

            internal void <>m__9C()
            {
                this.func.Call();
            }
        }

        [CompilerGenerated]
        private sealed class <System_Reflection_MemberFilter>c__AnonStorey87
        {
            internal LuaFunction func;

            internal bool <>m__9E(MemberInfo param0, object param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.PushObject(luaState, param0);
                LuaScriptMgr.PushVarObject(luaState, param1);
                this.func.PCall(oldTop, 2);
                object[] objArray = this.func.PopValues(oldTop);
                this.func.EndPCall(oldTop);
                return (bool) objArray[0];
            }
        }

        [CompilerGenerated]
        private sealed class <System_Reflection_TypeFilter>c__AnonStorey88
        {
            internal LuaFunction func;

            internal bool <>m__9F(Type param0, object param1)
            {
                int oldTop = this.func.BeginPCall();
                IntPtr luaState = this.func.GetLuaState();
                LuaScriptMgr.Push(luaState, param0);
                LuaScriptMgr.PushVarObject(luaState, param1);
                this.func.PCall(oldTop, 2);
                object[] objArray = this.func.PopValues(oldTop);
                this.func.EndPCall(oldTop);
                return (bool) objArray[0];
            }
        }

        [CompilerGenerated]
        private sealed class <UnityEngine_Events_UnityAction>c__AnonStorey86
        {
            internal LuaFunction func;

            internal void <>m__9D()
            {
                this.func.Call();
            }
        }

        private delegate Delegate DelegateValue(LuaFunction func);
    }
}

