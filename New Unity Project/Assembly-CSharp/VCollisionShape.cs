using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public abstract class VCollisionShape
{
    [NonSerialized, HideInInspector]
    public bool dirty = true;
    [NonSerialized, HideInInspector]
    public bool isBox;
    [NonSerialized, HideInInspector]
    public PoolObjHandle<ActorRoot> owner;
    private static DictionaryView<int, SCollisionComponent> s_componentCache = new DictionaryView<int, SCollisionComponent>();

    protected VCollisionShape()
    {
    }

    public abstract bool AcceptFowVisibilityCheck(COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr);
    public virtual void Born(ActorRoot actor)
    {
        actor.shape = this;
        this.owner = new PoolObjHandle<ActorRoot>(actor);
    }

    public static void ClearCache()
    {
        s_componentCache.Clear();
    }

    public void ConditionalUpdateShape()
    {
        if (this.dirty)
        {
            ActorRoot handle = this.owner.handle;
            if (this.isBox && (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet))
            {
                BulletWrapper actorControl = handle.ActorControl as BulletWrapper;
                if ((actorControl != null) && actorControl.GetMoveCollisiong())
                {
                    this.UpdateShape(handle.location, handle.forward, actorControl.GetMoveDelta());
                }
                else
                {
                    this.UpdateShape(handle.location, handle.forward);
                }
            }
            else
            {
                this.UpdateShape(handle.location, handle.forward);
            }
        }
    }

    public static VCollisionShape createFromCollider(GameObject gameObject)
    {
        DebugHelper.Assert((!Singleton<BattleLogic>.instance.isFighting || Singleton<GameLogic>.instance.bInLogicTick) || Singleton<FrameSynchr>.instance.isCmdExecuting);
        Collider component = gameObject.GetComponent<Collider>();
        if (component == null)
        {
            return null;
        }
        VCollisionShape shape = null;
        if (component is BoxCollider)
        {
            BoxCollider collider2 = component as BoxCollider;
            VCollisionBox box = new VCollisionBox();
            box.Pos = collider2.get_center();
            box.Size = collider2.get_size();
            return box;
        }
        if (component is CapsuleCollider)
        {
            CapsuleCollider collider3 = component as CapsuleCollider;
            VCollisionSphere sphere = new VCollisionSphere();
            Vector3 vector = collider3.get_center();
            vector.y -= collider3.get_height() * 0.5f;
            sphere.Pos = vector;
            VInt num = (VInt) collider3.get_radius();
            sphere.Radius = num.i;
            return sphere;
        }
        if (component is SphereCollider)
        {
            SphereCollider collider4 = component as SphereCollider;
            VCollisionSphere sphere2 = new VCollisionSphere();
            sphere2.Pos = collider4.get_center();
            VInt num2 = (VInt) collider4.get_radius();
            sphere2.Radius = num2.i;
            shape = sphere2;
        }
        return shape;
    }

    public abstract bool EdgeIntersects(VCollisionBox obb);
    public abstract bool EdgeIntersects(VCollisionCylinderSector cs);
    public bool EdgeIntersects(VCollisionShape shape)
    {
        bool flag = false;
        if (shape == null)
        {
            return flag;
        }
        switch (shape.GetShapeType())
        {
            case CollisionShapeType.Box:
                return this.EdgeIntersects((VCollisionBox) shape);

            case CollisionShapeType.CylinderSector:
                return this.EdgeIntersects((VCollisionCylinderSector) shape);
        }
        return this.EdgeIntersects((VCollisionSphere) shape);
    }

    public abstract bool EdgeIntersects(VCollisionSphere s);
    public abstract void GetAabb2D(out VInt2 lt, out VInt2 size);
    public abstract CollisionShapeType GetShapeType();
    public static VCollisionShape InitActorCollision(ActorRoot actor)
    {
        VCollisionShape shape = null;
        if (actor.shape == null)
        {
            SCollisionComponent component = null;
            if (!s_componentCache.TryGetValue(actor.gameObject.GetInstanceID(), out component))
            {
                component = actor.gameObject.GetComponent<SCollisionComponent>();
                s_componentCache[actor.gameObject.GetInstanceID()] = component;
            }
            if (component != null)
            {
                shape = component.CreateShape();
            }
            else if ((actor.CharInfo != null) && (actor.CharInfo.collisionType != CollisionShapeType.None))
            {
                shape = actor.CharInfo.CreateCollisionShape();
            }
            else
            {
                shape = createFromCollider(actor.gameObject);
            }
            if (shape != null)
            {
                shape.Born(actor);
            }
        }
        return actor.shape;
    }

    public static VCollisionShape InitActorCollision(ActorRoot actor, GameObject gameObj, string actionName)
    {
        VCollisionShape shape = null;
        if (actor.shape == null)
        {
            SCollisionComponent component = null;
            if ((null != gameObj) && !s_componentCache.TryGetValue(actor.gameObject.GetInstanceID(), out component))
            {
                component = gameObj.GetComponent<SCollisionComponent>();
                s_componentCache[actor.gameObject.GetInstanceID()] = component;
            }
            if (component != null)
            {
                shape = component.CreateShape();
            }
            else if ((actor.CharInfo != null) && (actor.CharInfo.collisionType != CollisionShapeType.None))
            {
                shape = actor.CharInfo.CreateCollisionShape();
            }
            else if (gameObj != null)
            {
                shape = createFromCollider(gameObj);
            }
            if (shape != null)
            {
                shape.Born(actor);
            }
        }
        return actor.shape;
    }

    public abstract bool Intersects(VCollisionBox obb);
    public abstract bool Intersects(VCollisionCylinderSector cs);
    public bool Intersects(VCollisionShape shape)
    {
        bool flag = false;
        if (shape == null)
        {
            return flag;
        }
        switch (shape.GetShapeType())
        {
            case CollisionShapeType.Box:
                return this.Intersects((VCollisionBox) shape);

            case CollisionShapeType.CylinderSector:
                return this.Intersects((VCollisionCylinderSector) shape);
        }
        return this.Intersects((VCollisionSphere) shape);
    }

    public abstract bool Intersects(VCollisionSphere s);
    public void OnEnable()
    {
        this.dirty = true;
        this.owner.Release();
    }

    public abstract void UpdateShape(VInt3 location, VInt3 forward);
    public abstract void UpdateShape(VInt3 location, VInt3 forward, int moveDelta);

    public abstract int AvgCollisionRadius { get; }
}

