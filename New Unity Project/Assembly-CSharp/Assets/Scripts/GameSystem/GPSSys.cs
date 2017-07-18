namespace Assets.Scripts.GameSystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class GPSSys : MonoSingleton<GPSSys>
    {
        [CompilerGenerated]
        private bool <bGetGPSData>k__BackingField;
        [CompilerGenerated]
        private bool <bRunning>k__BackingField;
        [CompilerGenerated]
        private int <iLatitude>k__BackingField;
        [CompilerGenerated]
        private int <iLongitude>k__BackingField;

        [DebuggerHidden]
        private IEnumerator _StartGPS()
        {
            <_StartGPS>c__Iterator27 iterator = new <_StartGPS>c__Iterator27();
            iterator.<>f__this = this;
            return iterator;
        }

        public void Clear()
        {
            this.bGetGPSData = false;
            this.bRunning = false;
            this.iLatitude = 0;
            this.iLongitude = 0;
        }

        protected override void Init()
        {
            this.bGetGPSData = false;
            this.bRunning = false;
        }

        public void StartGPS()
        {
            if (!this.bRunning)
            {
                this.bGetGPSData = false;
                this.bRunning = true;
                base.StartCoroutine(this._StartGPS());
            }
        }

        private void StopGPS()
        {
            Input.get_location().Stop();
            this.bRunning = false;
            this.bRunning = false;
            base.StopAllCoroutines();
        }

        public bool bGetGPSData
        {
            [CompilerGenerated]
            get
            {
                return this.<bGetGPSData>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<bGetGPSData>k__BackingField = value;
            }
        }

        public bool bRunning
        {
            [CompilerGenerated]
            get
            {
                return this.<bRunning>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<bRunning>k__BackingField = value;
            }
        }

        public int iLatitude
        {
            [CompilerGenerated]
            get
            {
                return this.<iLatitude>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<iLatitude>k__BackingField = value;
            }
        }

        public int iLongitude
        {
            [CompilerGenerated]
            get
            {
                return this.<iLongitude>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.<iLongitude>k__BackingField = value;
            }
        }

        [CompilerGenerated]
        private sealed class <_StartGPS>c__Iterator27 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal GPSSys <>f__this;
            internal int <maxWait>__0;

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
                        if (Input.get_location().get_isEnabledByUser())
                        {
                            break;
                        }
                        this.<>f__this.bRunning = true;
                        this.<>f__this.bGetGPSData = false;
                        this.$current = false;
                        this.$PC = 1;
                        goto Label_0221;

                    case 1:
                        break;

                    case 2:
                        goto Label_00C1;

                    case 3:
                        goto Label_0122;

                    case 4:
                        this.$PC = -1;
                        goto Label_021F;

                    default:
                        goto Label_021F;
                }
                Input.get_location().Start(5f, 5f);
                this.<maxWait>__0 = 20;
                while ((Input.get_location().get_status() == 1) && (this.<maxWait>__0 > 0))
                {
                    this.<>f__this.bRunning = true;
                    this.<>f__this.bGetGPSData = false;
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 2;
                    goto Label_0221;
                Label_00C1:
                    this.<maxWait>__0--;
                }
                if (this.<maxWait>__0 < 1)
                {
                    this.<>f__this.bRunning = true;
                    this.<>f__this.bGetGPSData = false;
                    this.$current = null;
                    this.$PC = 3;
                    goto Label_0221;
                }
            Label_0122:
                if (Input.get_location().get_status() == 3)
                {
                    this.<>f__this.bRunning = true;
                    this.<>f__this.bGetGPSData = false;
                    this.<>f__this.StopGPS();
                    this.$current = null;
                    this.$PC = 4;
                    goto Label_0221;
                }
                this.<>f__this.bGetGPSData = true;
                this.<>f__this.iLatitude = (int) (Input.get_location().get_lastData().get_latitude() * 1000000f);
                this.<>f__this.iLongitude = (int) (Input.get_location().get_lastData().get_longitude() * 1000000f);
                FriendSysNetCore.Send_Report_Clt_Location(this.<>f__this.iLongitude, this.<>f__this.iLatitude);
                FriendSysNetCore.Send_Search_LBS_Req((byte) Singleton<CFriendContoller>.instance.model.fileter, this.<>f__this.iLongitude, this.<>f__this.iLatitude, true);
                this.<>f__this.StopGPS();
            Label_021F:
                return false;
            Label_0221:
                return true;
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

