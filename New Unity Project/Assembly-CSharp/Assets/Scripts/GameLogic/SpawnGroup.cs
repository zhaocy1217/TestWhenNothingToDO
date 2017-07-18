namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class SpawnGroup : SpawnPoint
    {
        [FriendlyName("需要触发器")]
        public bool bTriggerSpawn;
        private SpawnPoint[] drawPoints;
        private Color GroupColor = new Color(0.8f, 0.1f, 0.1f, 0.8f);
        [HideInInspector]
        public int GroupId;
        [FriendlyName("失效时间")]
        public int InvalidTime;
        private bool m_bCountingSpawn;
        private int m_invalidTimer;
        private int m_spawnCounter;
        private int m_spawnTimer;
        public SpawnGroup[] NextGroups = new SpawnGroup[0];
        [SerializeField, HideInInspector]
        public PursuitInfo Pursuit = new PursuitInfo();
        [FriendlyName("生成间隔")]
        public int SpawnInternval = 0x2710;
        [FriendlyName("生成次数")]
        public int SpawnTimes;
        [FriendlyName("第一次生成延迟")]
        public int StartUpDelay = 0x1388;

        protected override void DecSpawnPointOver()
        {
            base.DecSpawnPointOver();
            if (base.m_spawnPointOver == 0)
            {
                this.m_bCountingSpawn = true;
                SGroupDeadEventParam prm = new SGroupDeadEventParam();
                prm.sg = this;
                Singleton<GameEventSys>.instance.SendEvent<SGroupDeadEventParam>(GameEventDef.Event_SpawnGroupDead, ref prm);
                if ((this.m_spawnCounter == 0) && (this.SpawnTimes > 0))
                {
                    this.Stop();
                    foreach (SpawnGroup group in this.NextGroups)
                    {
                        if (group != null)
                        {
                            group.TriggerStartUp();
                        }
                    }
                }
            }
        }

        private SpawnPoint[] FindChildrenPoints()
        {
            return base.GetComponentsInChildren<SpawnPoint>();
        }

        public int GetSpawnCounter()
        {
            return this.m_spawnCounter;
        }

        public int GetSpawnTimer()
        {
            return this.m_spawnTimer;
        }

        public bool IsCountingDown()
        {
            return this.m_bCountingSpawn;
        }

        private void OnDrawGizmos()
        {
            Gizmos.set_color(this.GroupColor);
            Gizmos.DrawSphere(base.get_transform().get_position(), 0.3f);
            this.drawPoints = this.FindChildrenPoints();
            if ((this.drawPoints != null) && (this.drawPoints.Length > 0))
            {
                Gizmos.set_color(this.GroupColor);
                for (int i = 0; i < (this.drawPoints.Length - 1); i++)
                {
                    Vector3 vector = this.drawPoints[0].get_gameObject().get_transform().get_position();
                    Vector3 vector2 = this.drawPoints[i + 1].get_gameObject().get_transform().get_position();
                    Vector3 vector3 = (vector2 - vector).get_normalized();
                    float num2 = (Vector3.Distance(vector2, vector) - this.drawPoints[i + 1].radius) - this.drawPoints[0].radius;
                    vector += (Vector3) (vector3 * this.drawPoints[0].radius);
                    vector2 = vector + ((Vector3) (vector3 * num2));
                    Gizmos.DrawLine(vector, vector2);
                    this.drawPoints[i + 1].PointColor = this.GroupColor;
                }
                Gizmos.DrawIcon(new Vector3(this.drawPoints[0].get_transform().get_position().x, this.drawPoints[0].get_transform().get_position().y + (this.drawPoints[0].radius * 3f), this.drawPoints[0].get_transform().get_position().z), "EndPoint", true);
            }
        }

        protected override void Start()
        {
            for (SpawnPoint point = base.NextPoint; point != null; point = point.NextPoint)
            {
                base.m_spawnPointList.Add(point);
                point.onAllDeadEvt += new SpawnPointAllDeadEvent(this.onSpawnPointAllDead);
            }
            base.Start();
        }

        public override void Startup()
        {
            if (!this.bTriggerSpawn && !base.isStartup)
            {
                this.m_spawnTimer = this.StartUpDelay;
                this.m_spawnCounter = this.SpawnTimes;
                this.m_bCountingSpawn = true;
                base.Startup();
            }
        }

        public void TriggerStartUp()
        {
            if (!base.isStartup)
            {
                this.m_spawnTimer = this.StartUpDelay;
                this.m_spawnCounter = this.SpawnTimes;
                this.m_bCountingSpawn = true;
                base.Startup();
            }
        }

        public override void UpdateLogic(int delta)
        {
            if (base.isStartup)
            {
                if (this.InvalidTime > 0)
                {
                    this.m_invalidTimer += delta;
                    if (this.m_invalidTimer >= this.InvalidTime)
                    {
                        this.Stop();
                        return;
                    }
                }
                if (this.m_bCountingSpawn && ((this.SpawnTimes <= 0) || (this.m_spawnCounter > 0)))
                {
                    this.m_spawnTimer -= delta;
                    if (this.m_spawnTimer <= 0)
                    {
                        this.m_spawnTimer = this.SpawnInternval;
                        base.DoSpawn(this.Pursuit);
                        this.m_bCountingSpawn = false;
                        this.m_spawnCounter--;
                    }
                }
            }
        }
    }
}

