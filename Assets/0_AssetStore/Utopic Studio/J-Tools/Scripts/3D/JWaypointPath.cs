using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(J.JWaypointPath))]
public class ResourceIndexInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        J.JWaypointPath WaypointPath = (J.JWaypointPath)target;
        if (GUILayout.Button("Bake points"))
        {
            WaypointPath.BakeSubObjects();
        }
    }
}
#endif

namespace J
{
    [Serializable]
    public class AgentReachedPointSignature : UnityEvent<UnityEngine.AI.NavMeshAgent, Vector3>
    {
    }

    [Serializable]
    public class AgentCompletedPathSignature : UnityEvent<UnityEngine.AI.NavMeshAgent>
    {
    }

    [AddComponentMenu("J/3D/JWaypointPath")]
    public class JWaypointPath : JBase
    {
        /// <summary>
        /// Called when an agent reaches a point on the nav movement list
        /// </summary>
        public AgentReachedPointSignature OnAgentReachedPoint;

        /// <summary>
        /// Called when an agent reaches the end of a path and will be removed from the list (only called on non-loopables)
        /// </summary>
        public AgentCompletedPathSignature OnAgentCompletedPath;

        /// <summary>
        /// The direction in which to navigate the array of points
        /// </summary>
        public enum Direction
        {
            Forward,
            Reverse
        }

        /// <summary>
        /// How to enter the navigation path
        /// </summary>
        public enum EntryMethod
        {
            Start,
            End,
            Closest
        }

        /// <summary>
        /// Holds the needed data for managing the waypoint queue
        /// </summary>
        [Serializable]
        public class WaypointQueueData
        {
            public WaypointQueueData(UnityEngine.AI.NavMeshAgent agent, Direction direction = Direction.Forward, bool shouldLoop = false, int initialTargetIndex = 0)
            {
                Agent = agent;
                MovementDirection = direction;
                ShouldLoopMovement = shouldLoop;
                CurrentTargetIndex = initialTargetIndex;

                //This variable is only stored for editing on the editor, we don't really use it.
                PathEntryMethod = EntryMethod.Start;
            }

            /// <summary>
            /// Mandatory agent used for navigation
            /// </summary>
            public UnityEngine.AI.NavMeshAgent Agent;

            /// <summary>
            /// Direction of movement for this queue
            /// </summary>
            public Direction MovementDirection;

            /// <summary>
            /// When looping, the movement will continue until stopped. If not looped, the movement will call a delegate and then erase this queue data.
            /// </summary>
            public bool ShouldLoopMovement;

            /// <summary>
            /// Index of the point in the list of waypoints this object is aiming for
            /// </summary>
            [NonSerialized]
            public int CurrentTargetIndex;

            /// <summary>
            /// Vairable used for initial configuration on editor, not really used after initialization.
            /// </summary>
            public EntryMethod PathEntryMethod;

            /// <summary>
            /// Used for validating the waypoint created internally.
            /// </summary>
            /// <returns> If the waypoint is valid or not</returns>
            public bool HasValidData()
            {
                return Agent != null;
            }

            /// <summary>
            /// Tests if the agent has completed its path and is awaiting orders
            /// </summary>
            /// <returns>If the agent has completed its movement towards the queued point.</returns>
            public bool HasCompletedMovement()
            {
                return !Agent.pathPending && !Agent.hasPath && Agent.remainingDistance <= Agent.stoppingDistance;
            }
        }

        /// <summary>
        /// Points that comprise the navigation path, in localspace
        /// </summary>
        [SerializeField]
        private Vector3[] Points;

        /// <summary>
        /// List of movements that need to be applied
        /// </summary>
        [SerializeField]
        private List<WaypointQueueData> MovementQueue = new List<WaypointQueueData>();

        /// <summary>
        /// Color of the gizmos to use on this path (for editor purposes only)
        /// </summary>
        public Color GizmosColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        //For painting the points and lines
        private void OnDrawGizmos()
        {
            Gizmos.color = GizmosColor;
            for(int i = 0; i < Points.Length; i++)
            {
                //Draw the sphere
                Vector3 PointWorldLocation = transform.TransformPoint(Points[i]);
                Gizmos.DrawSphere(PointWorldLocation, 1.0f);

                if(i > 0)
                {
                    Vector3 PreviousPointWorldLocation = transform.TransformPoint(Points[i - 1]);
                    Gizmos.DrawLine(PreviousPointWorldLocation, PointWorldLocation);
                }
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            if(Points.Length > 0)
            {
                //We need to initialize and validate each "editor" queue entry.
                for (int i = MovementQueue.Count - 1; i >= 0; i--)
                {
                    WaypointQueueData Data = MovementQueue[i];

                    if(Data.HasValidData())
                    {
                        //As this is an Editor placed point, we need to snap the initial position based on the preferred choice
                        switch (Data.PathEntryMethod)
                        {
                            case EntryMethod.Start:
                                Data.CurrentTargetIndex = 0;
                                break;
                            case EntryMethod.End:
                                Data.CurrentTargetIndex = Points.Length - 1;
                                break;
                            case EntryMethod.Closest:
                                Data.CurrentTargetIndex = ClosestIndexAtLocation(Data.Agent.transform.position);
                                break;
                            default:
                                break;
                        }

                        //After munching points, we need to ask the nav-movement to start. First navmovement is mandatory so the agent gets in track.
                        Data.Agent.SetDestination(transform.TransformPoint(Points[Data.CurrentTargetIndex]));
                    }
                    else
                    {
                        //Point is invalid, remove
                        MovementQueue.RemoveAt(i);
                    }
                }
            }
            else
            {
                //We have no points, we cannot enqueue any movement
                MovementQueue.Clear();
                Debug.LogWarning("Waypoint path tried to add movement queues but there are no points on the path.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Points.Length > 0)
            {
                //We move on a reversed for so we don't have issues with removing waypoints along the way
                for (int i = MovementQueue.Count - 1; i >= 0; i--)
                {
                    WaypointQueueData Data = MovementQueue[i];

                    if(Data.HasCompletedMovement())
                    {
                        Debug.Log("Waypoint queue at index " + i + " has reached its target point.");
                        OnAgentReachedPoint.Invoke(Data.Agent, Points[Data.CurrentTargetIndex]);

                        //Obtain the next point to use
                        int NextPoint = Data.MovementDirection == Direction.Forward ? Data.CurrentTargetIndex + 1 : Data.CurrentTargetIndex - 1;
                        if(NextPoint < 0 || NextPoint >= Points.Length)
                        {
                            //We just reached the end of our path
                            if(Data.ShouldLoopMovement)
                            {
                                Debug.Log("Waypoint queue at index " + i + " completed the path and will loop.");
                                NextPoint = Data.MovementDirection == Direction.Forward ? 0 : Points.Length - 1;
                            }
                            else
                            {
                                //Reached the end on a non looping movement
                                Debug.Log("Waypoint queue at index " + i + " completed the path.");
                                OnAgentCompletedPath.Invoke(Data.Agent);
                                MovementQueue.RemoveAt(i);
                                continue;
                            }
                        }

                        //Update the movement
                        Data.Agent.SetDestination(transform.TransformPoint(Points[NextPoint]));
                        Data.CurrentTargetIndex = NextPoint;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a movement to the queue
        /// </summary>
        /// <param name="agent">Navmesh agent that will drive the operation. </param>
        /// <param name="direction">Direction in which to move on the path. </param>
        /// <param name="shouldLoop">If the movement should continue or end when reaching the final point.</param>
        /// <param name="entryMethod">How to snap to the first point on the path.</param>
        public void EnqueueMovement(UnityEngine.AI.NavMeshAgent agent, Direction direction = Direction.Forward, bool shouldLoop = false, EntryMethod entryMethod = EntryMethod.Start)
        {
            if(Points.Length > 0)
            {
                int EntryIndex = entryMethod == EntryMethod.Start ? 0 : entryMethod == EntryMethod.End ? Points.Length - 1 : ClosestIndexAtLocation(agent.transform.position);
                WaypointQueueData Data = new WaypointQueueData(agent, direction, shouldLoop, EntryIndex);

                //Only add if everything is valid
                if(Data.HasValidData())
                {
                    MovementQueue.Add(Data);
                }
                else
                {
                    Debug.LogWarning("Tried to add an invalid waypoint movement data to the queue.");
                }
            }
            else
            {
                Debug.LogWarning("Tried to add a waypoint movement data point to a 0 point path");
            }
        }

        /// <summary>
        /// Obtains the closest navpoint from this poisition
        /// </summary>
        /// <param name="Position">Test position to use</param>
        /// <returns>index of the closes point in the array of waypoints.</returns>
        private int ClosestIndexAtLocation(Vector3 Position)
        {
            return 0; //@TODO: for now
        }

#if UNITY_EDITOR
        /// <summary>
        /// Uses the list of subobjects and the gameObject root to re-create the list of navigation points.
        /// </summary>
        public void BakeSubObjects()
        {
            List<Vector3> PointList = new List<Vector3>();

            //Initial point is always the root itself
            PointList.Add(Vector3.zero);

            //Then continue with subobjects
            Transform[] goTransforms = gameObject.GetComponentsInChildren<Transform>();
            foreach(Transform t in goTransforms)
            {
                if(t.gameObject != gameObject)
                {
                    PointList.Add(t.localPosition);
                }
            }

            Undo.RecordObject(this, "Bake nav positions");
            Points = PointList.ToArray();
            SceneView.RepaintAll();
        }
#endif
    }
}
