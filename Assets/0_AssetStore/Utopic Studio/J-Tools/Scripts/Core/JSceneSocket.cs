using UnityEngine;

namespace J
{


    /// <summary>
    /// A scene socket is a named component that is attached to a local transform, which can be used to denote or simulate
    /// sockets like UE4, although static.
    /// </summary>
    public class JSceneSocket : JBase
    {
        /// <summary>
        /// The position of this socket in local space.
        /// </summary>
        public Vector3 LocalPosition;

        /// <summary>
        /// The rotation of this socket in local space.
        /// </summary>
        public Quaternion Rotation
        {
            get { return Quaternion.Euler(EulerRotation); }
            set { EulerRotation = value.eulerAngles; }
        }

        /// <summary>
        /// The rotation of this socket in local space, represented as Euler angles.
        /// </summary>
        public Vector3 EulerRotation = Vector3.zero;

        /// <summary>
        /// Name of this socket.
        /// </summary>
        public string SocketName;

        public Vector3 GetWorldPosition()
        {
            return transform.TransformPoint(LocalPosition);
        }

        public Quaternion GetWorldRotation()
        {
            return transform.rotation * Rotation;
        }

    }
}