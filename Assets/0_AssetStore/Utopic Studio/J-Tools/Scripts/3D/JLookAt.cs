using UnityEngine;

namespace J
{

    [AddComponentMenu("J/3D/JLookAt")]
    public class JLookAt : JBase
    {
        

        [Tooltip("Dejar vacío para que este objeto sea el que mire a otro")]
        public Transform target;
        [Tooltip("Mirar a este objeto")]
        public Transform point;
        [Tooltip("Mirar al tag Player (Sólo si variable point está vacía)")]
        public bool lookAtPlayer = true;
        public bool allowVerticalRotation = true;
        public bool lookWithBackSide = false;
        [RangeAttribute(1f, 50f)]
        public float speed = 20f;

        Vector3 dir;


        private void Start()
        {
            if (!target)
                target = transform;
            if (lookAtPlayer && !point)
                point = (GameObject.FindGameObjectWithTag("Player") as GameObject).transform;

        }

        private void Update()
        {
            dir = point.position - target.position;
            if (lookWithBackSide)
                dir = -dir;
            if (!allowVerticalRotation)
                dir.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(dir);
            target.rotation = Quaternion.Slerp(target.rotation, rotation,
                Time.deltaTime * speed);
        }
    }

}