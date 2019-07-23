using UnityEngine;
using System.Collections;

namespace J
{
    [AddComponentMenu("J/Util/JAction")]
    public class JAction : JBase
    {

        [System.Serializable]
        struct JActionStruct
        {
            public float delay;
            public UnityEngine.Events.UnityEvent action;
        }

        [Tooltip("Para que las acciones se llamen al iniciar la escena")]
        [SerializeField]    bool callOnStart = false;
        [SerializeField]    UnityEngine.Events.UnityEvent action;
        [SerializeField]    JActionStruct[] delayedActions;
        //public JGlobalVariables global_vars;
        
        private void Start()
        {
            if (callOnStart)
                JCallActions();
        }

        private void Reset()
        {
            delayedActions = new JActionStruct[1];
            delayedActions[0].delay = 1f;
        }

        public void JCallActions()
        {
            action.Invoke();

            foreach (var jActionStruct in delayedActions)
            {
                StartCoroutine(_callAction(jActionStruct.action, jActionStruct.delay));
            }
        }

        IEnumerator _callAction(UnityEngine.Events.UnityEvent a, float delay)
        {
            yield return new WaitForSeconds(delay);
            a.Invoke();
        }


        
    }

}