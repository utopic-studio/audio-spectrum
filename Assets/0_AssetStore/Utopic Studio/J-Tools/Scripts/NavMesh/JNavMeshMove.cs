using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace J
{


    [AddComponentMenu("J/NavMesh/JNavMeshMove")]
    public class JNavMeshMove : JBase
    {


        [System.Serializable]
        public struct Personaje
        {
            public NavMeshAgent agent;
            public JNavMeshPoint posicion;
            public float rotationTime;
            public bool dontRotate;
            public bool callsOnArrive;
            [HideInInspector]
            public UnityEngine.Events.UnityEvent individualEvent;
            [HideInInspector]
            public bool hasArrived;
        }

        [Header("Mover al inicio?")]
        [SerializeField] bool moveOnStart;
        [Header("Personajes a mover")]
        [SerializeField] Personaje[] personajes;

        [Header("On Arrive Event")]
        [SerializeField] UnityEngine.Events.UnityEvent onArriveEvent;

        private bool moveAgentsCalled = false;
        private bool mainEventWasCalled = false;
        private bool allArrived = false;

        private List<int> agentsEventIndexes;
        private bool makeCall;



        private void OnValidate()
        {
            if (agentsEventIndexes == null)
                agentsEventIndexes = new List<int>();

            agentsEventIndexes.Clear();
            for (int i = 0; i < personajes.Length; i++)
            {
                if (personajes[i].callsOnArrive)
                    agentsEventIndexes.Add(i);
            }
        }

        private void Start()
        {
            if (moveOnStart)
                JMoveAgents();
        }

        private void Reset()
        {
            personajes = new Personaje[1];
            personajes[0].callsOnArrive = true;
        }

        private bool isAgentDoneWithHisPath(NavMeshAgent agent)
        {
            return !agent.hasPath && agent.remainingDistance <= agent.stoppingDistance + 0.1f && agent.velocity.sqrMagnitude <= 0.2f;
        }

        private void RotateAtArrival(Transform t, Transform directionTransform, float time)
        {
            JRotate jrotateComponent = directionTransform.gameObject.AddComponent<JRotate>();
            jrotateComponent.objToRotate = t;
            jrotateComponent.duration = time;
            if (jrotateComponent.duration > 0)
                jrotateComponent.JUseForwardLookOf(directionTransform);
            else
                jrotateComponent.JUseForwardLookOfInstant(directionTransform);
            Destroy(jrotateComponent, jrotateComponent.duration);
        }

        private void Update()
        {
            if (!allArrived)
            {
                // Individual Events and Rotation when object arrives
                allArrived = true;
                for (int i = 0; i < personajes.Length; i++)
                {
                    if (!personajes[i].hasArrived)
                        allArrived = false;

                    if (!personajes[i].hasArrived && isAgentDoneWithHisPath(personajes[i].agent))
                    {
                        personajes[i].hasArrived = true;

                        if (personajes[i].individualEvent != null)
                            personajes[i].individualEvent.Invoke();
                        if (!personajes[i].dontRotate)
                            RotateAtArrival(personajes[i].agent.transform, personajes[i].posicion.transform, personajes[i].rotationTime);
                    }
                }

                // Main Event
                if (moveAgentsCalled && !mainEventWasCalled)
                {
                    makeCall = true;
                    for (int j = 0; j < agentsEventIndexes.Count; j++)
                    {
                        int index = agentsEventIndexes[j];
                        if (!personajes[index].hasArrived)
                            makeCall = false;
                    }
                    if (makeCall && agentsEventIndexes.Count > 0)
                    {
                        CallOnArriveEvents();
                        mainEventWasCalled = true;
                    }

                }
            }

        }

        [System.Obsolete("Usar JMoveAgents")]
        public void MoveAgents()
        {
            this.JMoveAgents();
        }
        [System.Obsolete("Usar JMoveAgentsInstantly")]
        public void MoveAgentsInstantly()
        {
            this.JMoveAgentsInstantly();
        }
        public void JMoveAgents()
        {
            _MoveAgents(moveInstantly: false);
        }
        public void JMoveAgentsInstantly()
        {
            _MoveAgents(moveInstantly: true);
        }
        private void _MoveAgents(bool moveInstantly = false)
        {
            moveAgentsCalled = true;
            for (int i = 0; i < personajes.Length; i++)
            {
                if (personajes[i].agent.gameObject.activeInHierarchy && personajes[i].agent.isActiveAndEnabled)
                    if (moveInstantly)
                    {
                        personajes[i].agent.ResetPath();
                        personajes[i].agent.Warp(personajes[i].posicion.transform.position);
                    }
                    else
                    {
                        personajes[i].agent.SetDestination(personajes[i].posicion.transform.position);
                    }
            }
        }


        private void CallOnArriveEvents()
        {
            onArriveEvent.Invoke();
        }
    }

}