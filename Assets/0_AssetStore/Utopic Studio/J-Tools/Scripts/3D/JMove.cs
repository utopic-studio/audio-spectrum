using UnityEngine;

namespace J
{

	[AddComponentMenu("J/3D/JMove")]
	public class JMove : JBase
    {


		enum ModificationType
		{
			Local, Global
		}

    [Tooltip("Dejar vacío para mover este objeto")]
        [SerializeField] Transform target;
        [SerializeField]	ModificationType type;
        [SerializeField]	Vector3 direction = Vector3.forward;
		[SerializeField]	float speed = 1f;
		[SerializeField]	float acceleration = 0f;

		protected float moveFactorPrivate;
		protected float accelerationPrivate;

        private void Start()
        {
            if (!target)
                target = transform;
        }

        private void Update ()
        {
			accelerationPrivate = acceleration * Time.deltaTime;
			speed += accelerationPrivate;
			moveFactorPrivate = speed * Time.deltaTime;
			switch (type) {
			case ModificationType.Local:
				transform.position += transform.TransformVector (Vector3.Normalize (direction)) * moveFactorPrivate;
				break;
			case ModificationType.Global:
				transform.position += Vector3.Normalize(direction) * moveFactorPrivate;
				break;
			}
		}
	}

}