using UnityEditor;

using UnityEngine;
using System.Linq;

namespace J
{

    [AddComponentMenu("J/Animations/JAnimatorController")]
    public class JAnimatorController : JBase
    {
        public Animator animator;
        [HideInInspector]
        public string[] animatorState;
        [HideInInspector]
        public int _stateIndex = 0;

        private int[] animatorStateHash;
        private int animatorLastState;
        
      
        private void Reset()
        {
            if (!animator)
                animator = GetComponent<Animator>();
            if (animator)
            {
                var paramArr = animator.parameters.Where(param =>
                    //param.type == AnimatorControllerParameterType.Trigger || param.type == AnimatorControllerParameterType.Bool); //Parametros Bool no soportados aun para este script y el Animator
                    param.type == AnimatorControllerParameterType.Trigger);
                
                var param_count = paramArr.Count();

                animatorState = new string[param_count];
                animatorStateHash = new int[param_count];


                int i = 0;
                foreach (var item in paramArr)
                {
                    animatorState[i] = item.name;
                    animatorStateHash[i] = Animator.StringToHash(animatorState[i]);
                    i++;
                }
            }
        }


        private void OnValidate()
        {
            if (animator)
                Reset();
            
        }

        public void UpdateAnimator()
        {
            animator.SetTrigger(animatorStateHash[_stateIndex]);
        }

        private void LateUpdate()
        {
            int currentStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

            if (animatorLastState != currentStateHash)
                for (int i = 0; i < animatorState.Length; i++)
                {
                    if (animatorStateHash[i] == currentStateHash)
                        _stateIndex = i;

                }
            animatorLastState = currentStateHash;

            
        }

        private int CalculateStateIndex(string stateName)
        {
            int idx = -1;
            for (int i = 0; i < animatorState.Length; i++)
            {
                if (stateName == animatorState[i])
                    idx = i;
            }
            return idx;
        }
        


        
        public void JCallAnimation(string stateName)
        {
            int idx = CalculateStateIndex(stateName);
            if (idx >= 0)
            {
                _stateIndex = idx;
                UpdateAnimator();
            }
        }








    }




#if UNITY_EDITOR
    [CustomEditor(typeof(JAnimatorController))]
    public class JAnimatorControllerEditor : Editor
    {
        private new JAnimatorController target;
        private int _last_index = 0;

        void OnEnable()
        {
            target = (JAnimatorController)base.target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (target.animator)
                target._stateIndex = EditorGUILayout.Popup(target._stateIndex, target.animatorState);

            if (_last_index != target._stateIndex)
                target.UpdateAnimator();

            _last_index = target._stateIndex;

            // Save the changes back to the object
            EditorUtility.SetDirty(target);
        }
    }
#endif







}