using UnityEngine;

namespace J
{

    /// <summary>
    /// Usar J.followCurve para hacer Lerp fuera de la función Update()
    /// 
    /// Esta clase es de uso interno para realizar Lerp y debería ser
    /// llamada a travéz de J.JAutomaticLerp()
    /// </summary>
    public class JFollowCurve : JBase
    {

		public delegate void CurveDelegate(float x);

		[SerializeField]	AnimationCurve curve;
		[SerializeField]	float amplitudeFactor = 1f;
		[SerializeField]	int repeatCounter;
        [SerializeField]    float timeProgress;
        [SerializeField]    float curveCurrentTime;
        [SerializeField]    float curveCurrentValue;
        [SerializeField]    string createdBy;


        protected float duration = 1f;
        protected float timeVariable = 0f;
		protected float curveRealDuration;
		protected float curveStartTime;
		protected float modifyFactor;


		protected float curveValue;
		protected bool updateEnabled;
		protected bool reverse;
		protected CurveDelegate foo;





		public void beginFollowCurve(CurveDelegate d , float duration, float amplitude, int repeat, CurveType type, bool reverse, MonoBehaviour callingScript) {
			foo = d;
            updateEnabled = true;

			this.amplitudeFactor = amplitude;
            if (callingScript)
                this.createdBy = callingScript.name + " (Component " + callingScript.GetType().Name + ")";
			
			this.repeatCounter = repeat;

			this.curve = CurveGenerator.GenerateCurve (type);
            
			curveRealDuration = curve.keys [curve.length - 1].time - curve.keys [0].time;
			curveStartTime = curve.keys [0].time;

			this.reverse = reverse;

            this.duration = duration;

            
		}






		private void Start () {
			curve = CurveGenerator.GenerateCurve (CurveType.Linear);
		}

		private void Update () {
			if (updateEnabled) {
				float t = timeVariable + Time.deltaTime / duration;

				
				timeVariable = t % curveRealDuration;
				if (reverse)
					curveValue = curve.Evaluate (curveStartTime + curveRealDuration - timeVariable) * amplitudeFactor;
				else
					curveValue = curve.Evaluate (curveStartTime + timeVariable) * amplitudeFactor;


                if (repeatCounter > 0 && t > curveRealDuration)
                    repeatCounter--;


                if (repeatCounter == 0)
                {
                    updateEnabled = false;
                    if (reverse)
                        curveValue = Mathf.Min(curveValue, 0f);
                    else
                        curveValue = Mathf.Max(curveValue, amplitudeFactor);

                    foo(curveValue);
                }
                else
                {
                    foo(curveValue);

                    this.curveCurrentValue = curveValue;
                    this.curveCurrentTime = timeVariable;
                    this.timeProgress = timeVariable * this.duration;
                }
                    

			}
		}

	}

}