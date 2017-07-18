namespace Assets.Scripts.GameSystem
{
    using CSProtocol;
    using System;
    using System.Runtime.InteropServices;

    public class stFriendVerifyContent
    {
        public string content;
        public uint dwLogicWorldID;
        public COMDT_FRIEND_SOURCE friendSource;
        public int mentorType;
        public ulong ullUid;

        public stFriendVerifyContent(ulong ullUid, uint dwLogicWorldID, string content, COMDT_FRIEND_SOURCE friendSource, int mentor_type = 0)
        {
            this.ullUid = ullUid;
            this.dwLogicWorldID = dwLogicWorldID;
            this.content = content;
            this.friendSource = friendSource;
            this.mentorType = mentor_type;
        }
    }
}

