namespace Assets.Scripts.GameLogic
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class WaypointsHolder : FuncRegion
    {
        public float arrowHeadLength = 1f;
        public Color color = new Color(0f, 1f, 1f, 0.9f);
        public bool colorizeWaypoints = true;
        public int m_index;
        [HideInInspector]
        public Waypoint[] MyWaypoints;

        private Waypoint[] FindChildrenPoints()
        {
            Waypoint[] componentsInChildren = base.GetComponentsInChildren<Waypoint>();
            if (componentsInChildren.Length <= 0)
            {
                return null;
            }
            Waypoint[] waypointArray2 = new Waypoint[componentsInChildren.Length];
            IEnumerator enumerator = componentsInChildren.GetEnumerator();
            for (int i = 0; enumerator.MoveNext(); i++)
            {
                waypointArray2[i] = enumerator.Current as Waypoint;
                waypointArray2[i].AccessIndex = i;
            }
            return waypointArray2;
        }

        public Waypoint GetNextPoint(Waypoint InPoint)
        {
            if (InPoint == null)
            {
                return null;
            }
            return ((InPoint.AccessIndex >= (this.wayPoints.Length - 1)) ? null : this.wayPoints[InPoint.AccessIndex + 1]);
        }

        public Waypoint GetPrePoint(Waypoint InPoint)
        {
            if (InPoint == null)
            {
                return null;
            }
            return ((InPoint.AccessIndex <= 0) ? null : this.wayPoints[InPoint.AccessIndex - 1]);
        }

        private void OnDrawGizmos()
        {
            Waypoint[] waypointArray = this.FindChildrenPoints();
            if ((waypointArray != null) && (waypointArray.Length > 0))
            {
                Gizmos.set_color(this.color);
                for (int i = 0; i < (waypointArray.Length - 1); i++)
                {
                    Vector3 startPos = waypointArray[i].get_gameObject().get_transform().get_position();
                    Vector3 endPos = waypointArray[i + 1].get_gameObject().get_transform().get_position();
                    Vector3 vector3 = (endPos - startPos).get_normalized();
                    float num2 = (Vector3.Distance(endPos, startPos) - waypointArray[i + 1].radius) - waypointArray[i].radius;
                    startPos += (Vector3) (vector3 * waypointArray[i].radius);
                    endPos = startPos + ((Vector3) (vector3 * num2));
                    DrawArrowHelper.Draw(startPos, endPos, this.arrowHeadLength, 20f);
                    if (this.colorizeWaypoints)
                    {
                        waypointArray[i + 1].color = this.color;
                    }
                }
                Gizmos.DrawIcon(new Vector3(waypointArray[0].get_transform().get_position().x, waypointArray[0].get_transform().get_position().y + (waypointArray[0].radius * 3.5f), waypointArray[0].get_transform().get_position().z), "StartPoint", true);
                Gizmos.DrawIcon(new Vector3(waypointArray[waypointArray.Length - 1].get_transform().get_position().x, waypointArray[waypointArray.Length - 1].get_transform().get_position().y + (waypointArray[waypointArray.Length - 1].radius * 3.5f), waypointArray[waypointArray.Length - 1].get_transform().get_position().z), "EndPoint", true);
            }
        }

        private void Start()
        {
            if ((this.MyWaypoints == null) || (this.MyWaypoints.Length == 0))
            {
                this.MyWaypoints = this.FindChildrenPoints();
            }
        }

        public Waypoint endPoint
        {
            get
            {
                return (((this.wayPoints == null) || (this.wayPoints.Length <= 0)) ? null : this.wayPoints[this.wayPoints.Length - 1]);
            }
        }

        public Waypoint startPoint
        {
            get
            {
                return (((this.wayPoints == null) || (this.wayPoints.Length <= 0)) ? null : this.wayPoints[0]);
            }
        }

        public Waypoint[] wayPoints
        {
            get
            {
                return this.MyWaypoints;
            }
        }
    }
}

