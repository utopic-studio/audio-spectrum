using UnityEngine;

namespace J
{
    [AddComponentMenu("J/Animations/JAnimatorSpeedLink")]
    public class JAnimatorSpeedLink : JBase
    {
        
        public UnityEngine.AI.NavMeshAgent navMeshAgent;
        public Animator animator;
        [Header("Nombre del float en el blend tree del estado Default. Va de 0 a 1.")]
        public string parameterName = "Speed";

        private void Update()
        {
            float vel = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
            if (navMeshAgent && animator)
                animator.SetFloat(parameterName, vel);
        }

        private void Reset()
        {
            if (!animator)
                animator = GetComponent<Animator>();
            if (!navMeshAgent)
                navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            
        }

    }
}