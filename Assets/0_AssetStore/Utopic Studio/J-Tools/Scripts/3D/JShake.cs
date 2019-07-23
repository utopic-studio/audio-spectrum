using UnityEngine;

namespace J
{

    [AddComponentMenu("J/3D/JShake")]
    public class JShake : JBase
    {

        [SerializeField]    Transform target;
        [SerializeField]    float shakeAmount = 0.0f;
        [SerializeField]    float shakeDecay = 2.0f;

        void Start()
        {
            if (!target)
                target = transform;
        }

        void Update()
        {
            if (Time.deltaTime > 0.0f)
            {
                // set our position to a random value (this depends on knowing that the unshaken local-position is at the origin)
                transform.localPosition = target.localPosition + Random.insideUnitSphere * shakeAmount;
                // fade the shake amount towards zero
                shakeAmount = Mathf.Lerp(shakeAmount, 0.0f, shakeDecay * Time.deltaTime);
            }
        }

        public void Shake(float amount)
        {
            shakeAmount = amount;
        }
    }
}