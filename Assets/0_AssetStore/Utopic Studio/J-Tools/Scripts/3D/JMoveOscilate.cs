using UnityEngine;

namespace J
{
		
	[AddComponentMenu("J/3D/JMoveOscilate")]
	public class JMoveOscilate : JBase
    {

		enum ModificationType
		{
			Local, Global
		}
    [Tooltip("Dejar vacío para mover este objeto")]
        [SerializeField]    Transform target;
    [Tooltip("Explorar los presets de curvas - hay muchas")]
		[SerializeField]    AnimationCurve curve;
		[SerializeField]	ModificationType directionType;
		[SerializeField]	Vector3 direction = Vector3.forward;
		[SerializeField]	float distance = 1f;
		[SerializeField]	float duration = 1f;

		protected float timeVariable = 0f;
		protected Vector3 initialPosition;
		protected float curveDuration;
		protected float curveStartTime;
		protected float modifyFactor;

        private bool startHasRun = false;

		private void Start ()
        {
			UpdateCurveInfo ();
            initialPosition = transform.position;
            if (!target)
                target = transform;
            startHasRun = true;
		}
		private void OnValidate ()
        {
            if (startHasRun)
                UpdateCurveInfo ();
        }
        private void Reset()
        {
            InitializeMovementCurve();

        }

        private void Update ()
        {
			timeVariable = (timeVariable + Time.deltaTime / duration) % curveDuration;
			modifyFactor = curve.Evaluate (curveStartTime + timeVariable) * distance;
			switch (directionType) {
			case ModificationType.Local:
				transform.position = initialPosition + transform.TransformVector (Vector3.Normalize (direction)) * modifyFactor;
				break;
			case ModificationType.Global:
				transform.position = initialPosition + Vector3.Normalize(direction) * modifyFactor;
				break;
			}
		}

		protected void UpdateCurveInfo()
        {
			curveDuration = curve.keys [curve.length - 1].time - curve.keys [0].time;
			curveStartTime = curve.keys [0].time;
		}


        // Para inicializar el componente con alguna curva por defecto
        private void InitializeMovementCurve()
        {
            float tangentAngle = 1 / 0.1666f;
            curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, -tangentAngle, tangentAngle));
            curve.AddKey(new Keyframe(1 / 4, 1));
            curve.AddKey(new Keyframe(3 / 4, -1));
            curve.AddKey(new Keyframe(1, 0f, tangentAngle, -tangentAngle));

        }


    }


}