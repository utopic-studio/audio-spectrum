using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace J
{


    /// <summary>
    /// Specialized scene socket that can bind to another specific socket
    /// </summary>
    [AddComponentMenu("J/Interactions/BindSocket")]
    public class JBindSocket : JSceneSocket
    {
        private static float KINDA_SMALL_NUMBER = 0.001f;

        /// <summary>
        /// Called when the socket binds with another equally named socket on another gameObject.
        /// </summary>
        public UnityEvent OnBound;

        /// <summary>
        /// Radius of the binding for this bind to occur.
        /// </summary>
        public float BindRadius = 1.0f;

        /// <summary>
        /// Time in which two equally named sockets must stay at bind distance for them to snap.
        /// </summary>
        public float BindDelay = 1.0f;

        /// <summary>
        /// Scene socket that we want to bound to.
        /// </summary>
        public JSceneSocket TargetSocket = null;

        /// <summary>
        /// If this socket is bound to its target socket
        /// </summary>
        public bool Bound
        {
            get { return m_Bound; }
            private set { m_Bound = value; }
        }
        private bool m_Bound = false;

        /// <summary>
        /// If this bind sockets is enabled and should look for bindings.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// Lerp factor associated with binding movement
        /// </summary>
        public float SmoothFactor = 0.1f;

        /// <summary>
        /// Timer that counts up until the bind occurs
        /// </summary>
        private float m_CurrentBindTime;

        public void SetBindSocketEnabled(bool InEnabled)
        {
            Enabled = InEnabled;
        }

        private void Update()
        {
            if (Enabled && !m_Bound)
            {
                //Check if we're close enough to this socket to be bound
                if (InBoundDistance())
                {
                    m_CurrentBindTime += Time.deltaTime;

                    if (m_CurrentBindTime >= BindDelay)
                    {
                        Bound = true;
                        StartCoroutine(SmoothBind());

                        if (OnBound != null)
                        {
                            OnBound.Invoke();
                        }
                    }
                }
                else
                {
                    m_CurrentBindTime = 0.0f;
                }
            }
        }

        IEnumerator SmoothBind()
        {
            while (true)
            {
                //Start by lerping distance and rotation
                Vector3 DeltaPosition = TargetSocket.GetWorldPosition() - GetWorldPosition();

                //Take into consideration the socket's rotation
                Quaternion DeltaRotation = Quaternion.Inverse(transform.rotation) * TargetSocket.GetWorldRotation() * Quaternion.Inverse(Rotation);

                bool bSnapPosition = false;
                bool bSnapRotation = false;
                if (DeltaRotation.eulerAngles.sqrMagnitude < KINDA_SMALL_NUMBER)
                {
                    transform.rotation *= DeltaRotation;
                    bSnapRotation = true;
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * DeltaRotation, SmoothFactor);
                }

                //For easier movement, we rotate first and then move
                if (DeltaPosition.sqrMagnitude < KINDA_SMALL_NUMBER)
                {
                    transform.position += DeltaPosition;
                    bSnapPosition = true;
                }
                else if (bSnapRotation)
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + DeltaPosition, SmoothFactor);
                }

                //Check if we should keep on smoothing
                if (bSnapPosition && bSnapRotation)
                {
                    yield break;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        private bool InBoundDistance()
        {
            return TargetSocket ? Vector3.Distance(GetWorldPosition(), TargetSocket.GetWorldPosition()) <= BindRadius : false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.25f);
            Gizmos.DrawSphere(GetWorldPosition(), BindRadius);

            if (TargetSocket)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(GetWorldPosition(), TargetSocket.GetWorldPosition());
            }
        }
    }
}