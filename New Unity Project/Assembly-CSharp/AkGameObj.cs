using System;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("Wwise/AkGameObj")]
public class AkGameObj : MonoBehaviour
{
    public bool isEnvironmentAware = true;
    [SerializeField]
    private bool isStaticObject;
    public AkGameObjEnvironmentData m_envData;
    private AkGameObjPositionData m_posData;
    public AkGameObjPosOffsetData m_posOffsetData;

    private void AddAuxSend(GameObject in_AuxSendObject)
    {
        AkEnvironmentPortal component = in_AuxSendObject.GetComponent<AkEnvironmentPortal>();
        if (component != null)
        {
            this.m_envData.activePortals.Add(component);
            for (int i = 0; i < 2; i++)
            {
                if (component.environments[i] != null)
                {
                    int num2 = this.m_envData.activeEnvironments.BinarySearch(component.environments[i], AkEnvironment.s_compareByPriority);
                    if (num2 < 0)
                    {
                        this.m_envData.activeEnvironments.Insert(~num2, component.environments[i]);
                    }
                }
            }
            this.m_envData.auxSendValues = null;
            this.UpdateAuxSend();
        }
        else
        {
            AkEnvironment item = in_AuxSendObject.GetComponent<AkEnvironment>();
            if (item != null)
            {
                int num3 = this.m_envData.activeEnvironments.BinarySearch(item, AkEnvironment.s_compareByPriority);
                if (num3 < 0)
                {
                    this.m_envData.activeEnvironments.Insert(~num3, item);
                    this.m_envData.auxSendValues = null;
                    this.UpdateAuxSend();
                }
            }
        }
    }

    private void Awake()
    {
        if (!this.isStaticObject)
        {
            this.m_posData = new AkGameObjPositionData();
        }
        AkSoundEngine.RegisterGameObj(base.get_gameObject(), base.get_gameObject().get_name());
        Vector3 position = this.GetPosition();
        AkSoundEngine.SetObjectPosition(base.get_gameObject(), position.x, position.y, position.z, base.get_transform().get_forward().x, base.get_transform().get_forward().y, base.get_transform().get_forward().z);
        if (this.isEnvironmentAware)
        {
            this.m_envData = new AkGameObjEnvironmentData();
            this.AddAuxSend(base.get_gameObject());
        }
    }

    public Vector3 GetPosition()
    {
        if (this.m_posOffsetData != null)
        {
            Vector3 vector = base.get_transform().get_rotation() * this.m_posOffsetData.positionOffset;
            return (base.get_transform().get_position() + vector);
        }
        return base.get_transform().get_position();
    }

    private void OnDestroy()
    {
        foreach (AkUnityEventHandler handler in base.get_gameObject().GetComponents<AkUnityEventHandler>())
        {
            if (handler.triggerList.Contains(-358577003))
            {
                handler.DoDestroy();
            }
        }
        if (AkSoundEngine.IsInitialized())
        {
            AkSoundEngine.UnregisterGameObj(base.get_gameObject());
        }
    }

    private void OnEnable()
    {
        base.set_enabled(!this.isStaticObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.isEnvironmentAware)
        {
            this.AddAuxSend(other.get_gameObject());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.isEnvironmentAware)
        {
            AkEnvironmentPortal component = other.get_gameObject().GetComponent<AkEnvironmentPortal>();
            if (component != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    if ((component.environments[i] != null) && !base.GetComponent<Collider>().get_bounds().Intersects(component.environments[i].GetComponent<Collider>().get_bounds()))
                    {
                        this.m_envData.activeEnvironments.Remove(component.environments[i]);
                    }
                }
                this.m_envData.activePortals.Remove(component);
                this.m_envData.auxSendValues = null;
                this.UpdateAuxSend();
            }
            else
            {
                AkEnvironment item = other.get_gameObject().GetComponent<AkEnvironment>();
                if (item != null)
                {
                    for (int j = 0; j < this.m_envData.activePortals.Count; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            if (item == this.m_envData.activePortals[j].environments[k])
                            {
                                this.m_envData.auxSendValues = null;
                                this.UpdateAuxSend();
                                return;
                            }
                        }
                    }
                    this.m_envData.activeEnvironments.Remove(item);
                    this.m_envData.auxSendValues = null;
                    this.UpdateAuxSend();
                }
            }
        }
    }

    private void Update()
    {
        Vector3 position = this.GetPosition();
        if ((this.m_posData.position != position) || (this.m_posData.forward != base.get_transform().get_forward()))
        {
            this.m_posData.position = position;
            this.m_posData.forward = base.get_transform().get_forward();
            AkSoundEngine.SetObjectPosition(base.get_gameObject(), position.x, position.y, position.z, base.get_transform().get_forward().x, base.get_transform().get_forward().y, base.get_transform().get_forward().z);
            if (this.isEnvironmentAware)
            {
                this.UpdateAuxSend();
            }
        }
    }

    private void UpdateAuxSend()
    {
        if (this.m_envData.auxSendValues == null)
        {
            this.m_envData.auxSendValues = new AkAuxSendArray((this.m_envData.activeEnvironments.Count >= AkEnvironment.MAX_NB_ENVIRONMENTS) ? ((uint) AkEnvironment.MAX_NB_ENVIRONMENTS) : ((uint) this.m_envData.activeEnvironments.Count));
        }
        else
        {
            this.m_envData.auxSendValues.Reset();
        }
        for (int i = 0; i < this.m_envData.activePortals.Count; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                AkEnvironment item = this.m_envData.activePortals[i].environments[j];
                if ((item != null) && (this.m_envData.activeEnvironments.BinarySearch(item, AkEnvironment.s_compareByPriority) < AkEnvironment.MAX_NB_ENVIRONMENTS))
                {
                    this.m_envData.auxSendValues.Add(item.GetAuxBusID(), this.m_envData.activePortals[i].GetAuxSendValueForPosition(base.get_transform().get_position(), j));
                }
            }
        }
        if ((this.m_envData.auxSendValues.m_Count < AkEnvironment.MAX_NB_ENVIRONMENTS) && (this.m_envData.auxSendValues.m_Count < this.m_envData.activeEnvironments.Count))
        {
            ListView<AkEnvironment> view = new ListView<AkEnvironment>(this.m_envData.activeEnvironments);
            view.Sort(AkEnvironment.s_compareBySelectionAlgorithm);
            int num3 = Math.Min((int) (AkEnvironment.MAX_NB_ENVIRONMENTS - ((int) this.m_envData.auxSendValues.m_Count)), (int) (this.m_envData.activeEnvironments.Count - ((int) this.m_envData.auxSendValues.m_Count)));
            for (int k = 0; k < num3; k++)
            {
                if (!this.m_envData.auxSendValues.Contains(view[k].GetAuxBusID()) && (!view[k].isDefault || (k == 0)))
                {
                    this.m_envData.auxSendValues.Add(view[k].GetAuxBusID(), view[k].GetAuxSendValueForPosition(base.get_transform().get_position()));
                    if (view[k].excludeOthers)
                    {
                        break;
                    }
                }
            }
        }
        AkSoundEngine.SetGameObjectAuxSendValues(base.get_gameObject(), this.m_envData.auxSendValues, this.m_envData.auxSendValues.m_Count);
    }
}

