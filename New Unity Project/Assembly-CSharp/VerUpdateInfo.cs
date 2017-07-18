using Assets.Scripts.GameSystem;
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct VerUpdateInfo
{
    public LoginSvrInfo svrInfo;
    public enIIPSServerType svrType;
}

