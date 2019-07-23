using UnityEngine;

namespace J
{
	[AddComponentMenu("J/NavMesh/JNavMeshPatrol")]
	public class JNavMeshPatrol : JBase {

        [Tooltip("Dejar vacío para usar este objeto")]
        [SerializeField]    UnityEngine.AI.NavMeshAgent target;
        [SerializeField]	Transform[] points;
		[Tooltip("Aumentar si el objeto no sigue al siguiente waypoint")]
		[SerializeField]	float arrivedDistance = 0.5f;


		protected int destPoint = 0;


		void Start () {
			target = GetComponent<UnityEngine.AI.NavMeshAgent>();

			GotoNextPoint();
		}


		void GotoNextPoint() {
			// Returns if no points have been set up
			if (points.Length == 0)
				return;

			// Set the agent to go to the currently selected destination.
			target.destination = points[destPoint].position;

			// Choose the next point in the array as the destination,
			// cycling to the start if necessary.
			destPoint = (destPoint + 1) % points.Length;
		}


		void Update () {
			// Choose the next destination point when the agent gets
			// close to the current one.
			if (!target.pathPending && target.remainingDistance < arrivedDistance)
				GotoNextPoint();
		}
	}

}