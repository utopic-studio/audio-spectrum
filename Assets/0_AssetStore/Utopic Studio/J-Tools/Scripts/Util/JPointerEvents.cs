using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace J
{
    [AddComponentMenu("J/Util/JPointerEvents")]
    public class JPointerEvents : JBase, IPointerEnterHandler, IPointerExitHandler
    {
        private bool bPointerInside = false;
        public UnityEvent OnPointerStay;

        void OnDisable()
        {
            bPointerInside = false;
        }

        void Update()
        {
            if (bPointerInside && OnPointerStay != null)
                OnPointerStay.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            bPointerInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            bPointerInside = false;
        }
    }
}
