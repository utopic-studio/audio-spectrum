using UnityEngine;

namespace J
{

	[AddComponentMenu("J/3D/JScaleOscilate")]
	public class JScaleOscilate : JBase
    {
        
		[SerializeField]	AnimationCurve curve;
        [SerializeField]    Vector3 scaleVector = Vector3.one;
        [Range(0.01f, 60f)]
		[SerializeField]	float durationFactor = 1f;

		protected float timeVariable = 0f;
		protected Vector3 initialScale;
		protected float curveDuration;
		protected float curveStartTime;
		protected float curveValue;

		void Start () {
			UpdateCurveInfo ();

			initialScale = transform.localScale;
		}
		void OnValidate () {
			UpdateCurveInfo ();
        }
        private void Reset()
        {
            InitializeMovementCurve();
        }

        void Update () {
			timeVariable = (timeVariable + Time.deltaTime / durationFactor) % curveDuration;
			curveValue = curve.Evaluate (curveStartTime + timeVariable);
			transform.localScale = initialScale + scaleVector * curveValue;
			
		}

		protected void UpdateCurveInfo() {
			curveDuration = curve.keys [curve.length - 1].time - curve.keys [0].time;
			curveStartTime = curve.keys [0].time;
		}

        // Para inicializar el componente con alguna curva por defecto
        private void InitializeMovementCurve()
        {
            float tangentAngle = 1 / 0.1666f;
            curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, -tangentAngle, tangentAngle));
            curve.AddKey(new Keyframe( 1 / 4, 1));
            curve.AddKey(new Keyframe( 3 / 4, -1));
            curve.AddKey(new Keyframe(1, 0f, tangentAngle, -tangentAngle));

        }

    }


}