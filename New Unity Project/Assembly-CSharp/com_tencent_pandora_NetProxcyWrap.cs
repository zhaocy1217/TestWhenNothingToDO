using com.tencent.pandora;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

public class com_tencent_pandora_NetProxcyWrap
{
    private static Type classType = typeof(NetProxcy);

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int _Createcom_tencent_pandora_NetProxcy(IntPtr L)
    {
        if (LuaDLL.lua_gettop(L) == 0)
        {
            NetProxcy o = new NetProxcy();
            LuaScriptMgr.PushObject(L, o);
            return 1;
        }
        LuaDLL.luaL_error(L, "invalid arguments to method: com.tencent.pandora.NetProxcy.New");
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int DoCallBack(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        int iFlag = (int) LuaScriptMgr.GetNumber(L, 3);
        NetProxcy.DoCallBack(luaString, number, iFlag);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_haveSOData(IntPtr L)
    {
        LuaScriptMgr.Push(L, NetProxcy.haveSOData);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_instance(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetProxcy.instance);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_listInfoSO(IntPtr L)
    {
        LuaScriptMgr.PushObject(L, NetProxcy.listInfoSO);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_mds_op(IntPtr L)
    {
        LuaScriptMgr.Push(L, NetProxcy.mds_op);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int get_tokenkey(IntPtr L)
    {
        LuaScriptMgr.Push(L, NetProxcy.tokenkey);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetActionList(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        NetProxcy.GetActionList(LuaScriptMgr.GetLuaString(L, 1));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetActionListCache(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        NetProxcy.GetActionListCache();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetCallBackData(IntPtr L)
    {
        <GetCallBackData>c__AnonStorey58 storey = new <GetCallBackData>c__AnonStorey58();
        storey.L = L;
        LuaScriptMgr.CheckArgsCount(storey.L, 1);
        NetProxcy.CallBack callFunConnect = null;
        if (LuaDLL.lua_type(storey.L, 1) != LuaTypes.LUA_TFUNCTION)
        {
            callFunConnect = (NetProxcy.CallBack) LuaScriptMgr.GetNetObject(storey.L, 1, typeof(NetProxcy.CallBack));
        }
        else
        {
            <GetCallBackData>c__AnonStorey57 storey2 = new <GetCallBackData>c__AnonStorey57();
            storey2.<>f__ref$88 = storey;
            storey2.func = LuaScriptMgr.GetLuaFunction(storey.L, 1);
            callFunConnect = new NetProxcy.CallBack(storey2.<>m__27);
        }
        int callBackData = NetProxcy.GetCallBackData(callFunConnect);
        LuaScriptMgr.Push(storey.L, callBackData);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetClassType(IntPtr L)
    {
        LuaScriptMgr.Push(L, classType);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GetInstance(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        NetProxcy instance = NetProxcy.GetInstance();
        LuaScriptMgr.PushObject(L, instance);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int GpmPay(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        int iFlag = (int) LuaScriptMgr.GetNumber(L, 3);
        int d = NetProxcy.GpmPay(luaString, number, iFlag);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InitSocket(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 1);
        ((NetProxcy) LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.NetProxcy")).InitSocket();
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int InitTcpSocket(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 5);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        int iLength = (int) LuaScriptMgr.GetNumber(L, 3);
        int iLoginFlag = (int) LuaScriptMgr.GetNumber(L, 4);
        int iGetListFlag = (int) LuaScriptMgr.GetNumber(L, 5);
        int d = NetProxcy.InitTcpSocket(number, luaString, iLength, iLoginFlag, iGetListFlag);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int midaspayCallBack(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        NetProxcy proxcy = (NetProxcy) LuaScriptMgr.GetNetObjectSelf(L, 1, "com.tencent.pandora.NetProxcy");
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        proxcy.midaspayCallBack(luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int parseActionData(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 2);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        bool boolean = LuaScriptMgr.GetBoolean(L, 2);
        int d = NetProxcy.parseActionData(luaString, boolean);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int PraseConnect(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 3);
        string luaString = LuaScriptMgr.GetLuaString(L, 1);
        int number = (int) LuaScriptMgr.GetNumber(L, 2);
        int iFlag = (int) LuaScriptMgr.GetNumber(L, 3);
        int d = NetProxcy.PraseConnect(luaString, number, iFlag);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    public static void Register(IntPtr L)
    {
        LuaMethod[] regs = new LuaMethod[] { 
            new LuaMethod("U3dCloseSocket", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.U3dCloseSocket)), new LuaMethod("GpmPay", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.GpmPay)), new LuaMethod("GetCallBackData", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.GetCallBackData)), new LuaMethod("InitTcpSocket", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.InitTcpSocket)), new LuaMethod("SendPdrLibCmd", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.SendPdrLibCmd)), new LuaMethod("GetInstance", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.GetInstance)), new LuaMethod("SendPandoraLibCmd", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.SendPandoraLibCmd)), new LuaMethod("DoCallBack", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.DoCallBack)), new LuaMethod("midaspayCallBack", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.midaspayCallBack)), new LuaMethod("PraseConnect", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.PraseConnect)), new LuaMethod("InitSocket", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.InitSocket)), new LuaMethod("GetActionList", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.GetActionList)), new LuaMethod("StaticReport", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.StaticReport)), new LuaMethod("SendLogReport", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.SendLogReport)), new LuaMethod("GetActionListCache", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.GetActionListCache)), new LuaMethod("parseActionData", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.parseActionData)), 
            new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap._Createcom_tencent_pandora_NetProxcy)), new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.GetClassType))
         };
        LuaField[] fields = new LuaField[] { new LuaField("instance", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.get_instance), new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.set_instance)), new LuaField("haveSOData", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.get_haveSOData), new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.set_haveSOData)), new LuaField("listInfoSO", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.get_listInfoSO), new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.set_listInfoSO)), new LuaField("mds_op", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.get_mds_op), new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.set_mds_op)), new LuaField("tokenkey", new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.get_tokenkey), new LuaCSFunction(com_tencent_pandora_NetProxcyWrap.set_tokenkey)) };
        LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.NetProxcy", typeof(NetProxcy), regs, fields, typeof(object));
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SendLogReport(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        int reportType = (int) LuaScriptMgr.GetNumber(L, 2);
        int toReturnCode = (int) LuaScriptMgr.GetNumber(L, 3);
        string luaString = LuaScriptMgr.GetLuaString(L, 4);
        NetProxcy.SendLogReport(number, reportType, toReturnCode, luaString);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SendPandoraLibCmd(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        int iLength = (int) LuaScriptMgr.GetNumber(L, 3);
        int iFlag = (int) LuaScriptMgr.GetNumber(L, 4);
        NetProxcy.SendPandoraLibCmd(number, luaString, iLength, iFlag);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int SendPdrLibCmd(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 4);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        string luaString = LuaScriptMgr.GetLuaString(L, 2);
        int iLength = (int) LuaScriptMgr.GetNumber(L, 3);
        int iFlag = (int) LuaScriptMgr.GetNumber(L, 4);
        int d = NetProxcy.SendPdrLibCmd(number, luaString, iLength, iFlag);
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_haveSOData(IntPtr L)
    {
        NetProxcy.haveSOData = LuaScriptMgr.GetBoolean(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_instance(IntPtr L)
    {
        NetProxcy.instance = (NetProxcy) LuaScriptMgr.GetNetObject(L, 3, typeof(NetProxcy));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_listInfoSO(IntPtr L)
    {
        NetProxcy.listInfoSO = (Queue) LuaScriptMgr.GetNetObject(L, 3, typeof(Queue));
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_mds_op(IntPtr L)
    {
        NetProxcy.mds_op = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int set_tokenkey(IntPtr L)
    {
        NetProxcy.tokenkey = LuaScriptMgr.GetString(L, 3);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int StaticReport(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 10);
        int number = (int) LuaScriptMgr.GetNumber(L, 1);
        int iChannelId = (int) LuaScriptMgr.GetNumber(L, 2);
        int iActionId = (int) LuaScriptMgr.GetNumber(L, 3);
        int iReportType = (int) LuaScriptMgr.GetNumber(L, 4);
        int iJumpType = (int) LuaScriptMgr.GetNumber(L, 5);
        string luaString = LuaScriptMgr.GetLuaString(L, 6);
        string strGoodsId = LuaScriptMgr.GetLuaString(L, 7);
        int iGoodsNum = (int) LuaScriptMgr.GetNumber(L, 8);
        int iGoodFee = (int) LuaScriptMgr.GetNumber(L, 9);
        int iMoneyType = (int) LuaScriptMgr.GetNumber(L, 10);
        NetProxcy.StaticReport(number, iChannelId, iActionId, iReportType, iJumpType, luaString, strGoodsId, iGoodsNum, iGoodFee, iMoneyType);
        return 0;
    }

    [MonoPInvokeCallback(typeof(LuaCSFunction))]
    private static int U3dCloseSocket(IntPtr L)
    {
        LuaScriptMgr.CheckArgsCount(L, 0);
        int d = NetProxcy.U3dCloseSocket();
        LuaScriptMgr.Push(L, d);
        return 1;
    }

    [CompilerGenerated]
    private sealed class <GetCallBackData>c__AnonStorey57
    {
        internal com_tencent_pandora_NetProxcyWrap.<GetCallBackData>c__AnonStorey58 <>f__ref$88;
        internal LuaFunction func;

        internal void <>m__27(string param0, int param1, int param2)
        {
            int oldTop = this.func.BeginPCall();
            LuaScriptMgr.Push(this.<>f__ref$88.L, param0);
            LuaScriptMgr.Push(this.<>f__ref$88.L, param1);
            LuaScriptMgr.Push(this.<>f__ref$88.L, param2);
            this.func.PCall(oldTop, 3);
            this.func.EndPCall(oldTop);
        }
    }

    [CompilerGenerated]
    private sealed class <GetCallBackData>c__AnonStorey58
    {
        internal IntPtr L;
    }
}

