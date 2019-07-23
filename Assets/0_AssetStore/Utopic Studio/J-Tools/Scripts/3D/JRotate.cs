using UnityEngine;

namespace J
{

    [AddComponentMenu("J/3D/JRotate")]
    public class JRotate : JBase
    {
        [Header("(Llama la rotación con JAction)")]
        [Tooltip("Dejar vacío para rotar este objeto")]
        public Transform objToRotate;
        public float duration = 1f;
        [SerializeField] bool allowVerticalRotation = false;
        [SerializeField] bool lookWithBackSide = false;

        private bool rotating = false;
        private Quaternion _lookRotation;
        private Quaternion _initialRotation;
        private float _lerp_float;

        private void Start()
        {
            if (!objToRotate)
                objToRotate = transform;
        }

        public void JUseForwardLookOf(Transform t)
        {
            _LookAt(objToRotate.position + t.forward, false, objToRotate);
        }
        public void JUseForwardLookOfInstant(Transform t)
        {
            _LookAt(objToRotate.position + t.forward, true, objToRotate);
        }
        public void JLookAt(Transform t)
        {
            _LookAt(t.position, false, objToRotate);
        }

        public void JLookAtInstant(Transform t)
        {
            _LookAt(t.position, true, objToRotate);
        }

        private void _LookAt(Vector3 point, bool instant, Transform targetX = null)
        {
            
            _initialRotation = targetX.rotation;

            Vector3 target_pos = targetX.position;     //posicion de objeto a ser rotado
            Vector3 _direction = (point - target_pos).normalized;
            if (!allowVerticalRotation)
                _direction.y = 0;
            _lookRotation = Quaternion.LookRotation(_direction);

            if (lookWithBackSide)
                _lookRotation = Quaternion.EulerAngles(_lookRotation.x, -_lookRotation.y, _lookRotation.z);

            if (instant)
                targetX.rotation = Quaternion.RotateTowards(_initialRotation, _lookRotation, 360f);
            else
                J.Instance.JLerp(x =>
                {
                    targetX.rotation = Quaternion.Lerp(_initialRotation, _lookRotation, x);
                },
                duration, callingScript: this);
             
        }
    }

}