namespace Assets.Scripts.GameLogic
{
    using Assets.Scripts.Common;
    using System;
    using System.Collections.Generic;

    public class TeleportTargetSearcher : Singleton<TeleportTargetSearcher>
    {
        private PoolObjHandle<ActorRoot> curActorPtr;
        private TargetPropertyLessEqualFilter eyeFiler = new TargetPropertyLessEqualFilter();
        private List<ActorRoot> eyeList = new List<ActorRoot>();
        private TargetPropertyLessEqualFilter heroFilter = new TargetPropertyLessEqualFilter();
        private List<ActorRoot> heroList = new List<ActorRoot>();
        private SceneManagement.Process NearestHandler;
        private TargetPropertyLessEqualFilter organFiler = new TargetPropertyLessEqualFilter();
        private List<ActorRoot> organList = new List<ActorRoot>();
        private VInt3 searchPosition;
        private int searchRadius;
        private uint searchTypeMask;

        private void Clear()
        {
            this.curActorPtr.Release();
            this.heroList.Clear();
            this.organList.Clear();
            this.eyeList.Clear();
            this.heroFilter.Initial(this.heroList, ulong.MaxValue);
            this.organFiler.Initial(this.organList, ulong.MaxValue);
            this.eyeFiler.Initial(this.eyeList, ulong.MaxValue);
        }

        private void FilterNearestCanTeleportActorByPosition(ref PoolObjHandle<ActorRoot> _actorPtr)
        {
            if (this.curActorPtr.handle.CanTeleport((ActorRoot) _actorPtr) && (this.curActorPtr != _actorPtr))
            {
                if (TypeSearchCondition.Fit((ActorRoot) _actorPtr, ActorTypeDef.Actor_Type_Hero))
                {
                    if (DistanceSearchCondition.Fit(this.searchPosition, (ActorRoot) _actorPtr, this.searchRadius))
                    {
                        this.heroFilter.Searcher(this.searchPosition, (ActorRoot) _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
                    }
                }
                else if (TypeSearchCondition.Fit((ActorRoot) _actorPtr, ActorTypeDef.Actor_Type_Organ))
                {
                    if (DistanceSearchCondition.Fit(this.searchPosition, (ActorRoot) _actorPtr, this.searchRadius))
                    {
                        this.organFiler.Searcher(this.searchPosition, (ActorRoot) _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
                    }
                }
                else if (TypeSearchCondition.Fit((ActorRoot) _actorPtr, ActorTypeDef.Actor_Type_EYE, false) && DistanceSearchCondition.Fit(this.searchPosition, (ActorRoot) _actorPtr, this.searchRadius))
                {
                    this.eyeFiler.Searcher(this.searchPosition, (ActorRoot) _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
                }
            }
        }

        private uint GetSearchPriorityTarget()
        {
            if (this.heroList.Count >= 1)
            {
                return this.heroList[0].ObjID;
            }
            if (this.organList.Count >= 1)
            {
                return this.organList[0].ObjID;
            }
            if (this.eyeList.Count >= 1)
            {
                return this.eyeList[0].ObjID;
            }
            return 0;
        }

        public uint SearchNearestCanTeleportTarget(ref PoolObjHandle<ActorRoot> _actorPtr, VInt3 _position, int _srchR)
        {
            this.Clear();
            this.curActorPtr = _actorPtr;
            this.searchRadius = _srchR;
            this.searchPosition = _position;
            this.NearestHandler = new SceneManagement.Process(this.FilterNearestCanTeleportActorByPosition);
            SceneManagement instance = Singleton<SceneManagement>.GetInstance();
            SceneManagement.Coordinate coord = new SceneManagement.Coordinate();
            instance.GetCoord_Center(ref coord, _position.xz, _srchR);
            instance.UpdateDirtyNodes();
            instance.ForeachActors(coord, this.NearestHandler);
            return this.GetSearchPriorityTarget();
        }
    }
}

