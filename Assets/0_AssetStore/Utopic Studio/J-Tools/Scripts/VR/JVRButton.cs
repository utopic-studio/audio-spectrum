using UnityEngine;

namespace J
{


    [AddComponentMenu("J/VR/JVRButton")]
    public class JVRButton : JBase, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
    {

        [SerializeField]    UnityEngine.EventSystems.EventTrigger callOnClick;
        [Min(0f)]
        [SerializeField]    float waitTime = 1.5f;
        [SerializeField]    AudioClip clickSound;
        [Range(0f, 1f)]
        [SerializeField]    float vol = 0.7f;

        private float waitTimePrivate = 0f;

        private void Start()
        {
            if (!callOnClick)
                callOnClick = GetComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        private void Update()
        {
            if (waitTimePrivate > 0f)
            {
                waitTimePrivate -= Time.deltaTime;
                if (waitTimePrivate <= 0f)
                {
                    ButtonClickAfterCooldown();
                }
            }
        }

        #region IPointerEnterHandler implementation

        public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            waitTimePrivate = waitTime;
        }

        #endregion

        #region IPointerExitHandler implementation

        public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            waitTimePrivate = 0f;
        }

        #endregion


        private void ButtonClickAfterCooldown()
        {
            callOnClick.OnPointerClick(null);
            PlayClickSound(clickSound);
        }
        private void PlayClickSound(AudioClip clip)
        {
            if(clip)
                J.Instance.JPlaySound(clip, vol);
        }

    }
}