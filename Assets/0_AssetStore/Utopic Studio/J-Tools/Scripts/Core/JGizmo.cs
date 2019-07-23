using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace J
{

    public class JGizmo : JBase
    {
        public string gizmoName = "sticky_arrow";
        public string listOfGizmos = "https://unitylist.com/p/5c3/Unity-editor-icons";

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Vector3 PointWorldLocation = transform.position;   
            Gizmos.DrawIcon(PointWorldLocation, gizmoName);
#endif
        }
    }

}

