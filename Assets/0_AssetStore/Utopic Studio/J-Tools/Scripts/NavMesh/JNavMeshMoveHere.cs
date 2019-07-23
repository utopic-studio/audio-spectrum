using UnityEngine;

namespace J
{
    public class JNavMeshMoveHere : JBase
    {

        public string tagFind = "Player";
        public UnityEngine.AI.NavMeshAgent agent;
        [Tooltip("If set to None, nav mesh agents walks to this object")]
        public Transform destination;


        public void JMoveAgent()
        {
            if (!agent && tag.Trim().Length > 0)
                agent = GameObject.FindGameObjectWithTag(tagFind).GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (destination)
                agent.destination = destination.position;
            else
                agent.destination = transform.position;
        }

    }
}


