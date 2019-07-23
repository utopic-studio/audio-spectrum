using UnityEngine;

namespace J
{

	[AddComponentMenu("J/3D/JMoveToPoint")]
	public class JMoveToPoint : JBase
    {

    [Tooltip("Dejar vacío para mover este objeto")]
        [SerializeField]    Transform target;
    [Tooltip("Destination transform")]
        [SerializeField]    Transform point;
        [SerializeField] bool pointMoves = false;
        [SerializeField]	float speed = 1f;
		[SerializeField]	float acceleration = 0f;
        [SerializeField]    float arriveThreshold = 0.05f;


        protected float moveFactorPrivate;
		protected float accelerationPrivate;
        protected Vector3 direction;
        protected bool hasReachedDestination = false;

        private void Start()
        {
            if (!target)
                target = transform;

            direction = _CalculateDirection();
        }

        private Vector3 _CalculateDirection()
        {
            return point.position - target.position;
        }

        private void Update ()
        {
            if (!hasReachedDestination)
            {
                if (pointMoves)
                    direction = _CalculateDirection();

                accelerationPrivate = acceleration * Time.deltaTime;
                speed += accelerationPrivate;
                moveFactorPrivate = speed * Time.deltaTime;
                //transform.position += transform.TransformVector(Vector3.Normalize(direction)) * moveFactorPrivate;
                transform.position += Vector3.Normalize(direction) * moveFactorPrivate;

                if (_CalculateDirection().magnitude < arriveThreshold)
                    hasReachedDestination = true;
            }
        }
	}

}