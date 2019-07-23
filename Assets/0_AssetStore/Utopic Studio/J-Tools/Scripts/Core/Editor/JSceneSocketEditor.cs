using UnityEngine;
using UnityEditor;

namespace J
{

    [CustomEditor(typeof(JSceneSocket)), CanEditMultipleObjects]
    public class JSceneSocketEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {
            JSceneSocket Socket = (JSceneSocket)target;

            EditorGUI.BeginChangeCheck();
            if (Tools.current == Tool.Move)
            {
                Vector3 WorldPosition = Handles.PositionHandle(Socket.transform.TransformPoint(Socket.LocalPosition), Socket.transform.rotation * Socket.Rotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Socket, "Change SceneSocket Position");
                    Socket.LocalPosition = Socket.transform.InverseTransformPoint(WorldPosition);
                }
            }
            else if (Tools.current == Tool.Rotate)
            {
                Quaternion WorldRotation = Handles.RotationHandle(Socket.transform.rotation * Socket.Rotation, Socket.transform.TransformPoint(Socket.LocalPosition));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(Socket, "Change SceneSocket Position");
                    Socket.Rotation = Quaternion.Inverse(Socket.transform.rotation) * WorldRotation;
                }
            }
        }
    }
}