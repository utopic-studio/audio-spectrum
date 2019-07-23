using UnityEngine;

namespace J
{

    [AddComponentMenu("J/UI/JFadeUI")]
    public class JFadeUI : JBase
    {

        enum UIAlphaStartMode { invisible, visible, visibleFadeIn, invisibleFadeOut }

        [Tooltip("Dejar vacío si el CanvasGroup está en este objeto")]
        [SerializeField] CanvasGroup[] target;
        [SerializeField] UIAlphaStartMode mode = UIAlphaStartMode.visible;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] UnityEngine.Events.UnityEvent DoAfterFadeIn;
        [SerializeField] UnityEngine.Events.UnityEvent DoAfterFadeOut;

        private bool _postStart = false;
        UIAlphaStartMode _mode;

        private void OnValidate()
        {
            if (_postStart)
            {
                UpdateMode();
            }
        }
        private void Reset()
        {
            target = new CanvasGroup[1];
            CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
            if (cg)
                target[0] = cg;
        }
        private void Start()
        {
            _postStart = true;

            UpdateMode();
        }


        public void JShow()
        {
            mode = UIAlphaStartMode.visibleFadeIn;
            UpdateMode();
        }
        public void JHide()
        {
            mode = UIAlphaStartMode.invisibleFadeOut;
            UpdateMode();
        }
        public void JShowInstantly()
        {
            mode = UIAlphaStartMode.visible;
            UpdateMode();
        }
        public void JHideInstantly()
        {
            mode = UIAlphaStartMode.invisible;
            UpdateMode();
        }

        // Se mantiene por compatibilidad
        public void Show()
        {
            JShow();
        }
        // Se mantiene por compatibilidad
        public void Hide()
        {
            JHide();
        }
        // Se mantiene por compatibilidad
        public void ShowInstantly()
        {
            JShowInstantly();
        }
        // Se mantiene por compatibilidad
        public void HideInstantly()
        {
            JHideInstantly();
        }

        private void UpdateMode()
        {
            if (ModeChanged())
            {
                switch (mode)
                {
                    case UIAlphaStartMode.invisible:
                        _HideInstantly();
                        break;
                    case UIAlphaStartMode.visible:
                        _ShowInstantly();
                        break;
                    case UIAlphaStartMode.visibleFadeIn:
                        _Show();
                        break;
                    case UIAlphaStartMode.invisibleFadeOut:
                        _Hide();
                        break;
                    default:
                        break;
                }
                _mode = mode;
            }
        }

        private bool ModeChanged()
        {
            return mode != _mode;
        }

        private void DoOnFadeInEnded()
        {
            DoAfterFadeIn.Invoke();
        }
        private void DoOnFadeOutEnded()
        {
            DoAfterFadeOut.Invoke();
        }


        private void _Show()
        {
            foreach (var g in target)
                this._Fade(g, duration: fadeInTime, reverse: false);
            Invoke("DoOnFadeInEnded", fadeInTime);
        }
        private void _Hide()
        {
            foreach (var g in target)
                this._Fade(g, duration: fadeOutTime, reverse: true);
            Invoke("DoOnFadeOutEnded", fadeOutTime);
        }
        private void _ShowInstantly()
        {
            foreach (var g in target)
                g.alpha = 1f;
            Invoke("DoOnFadeInEnded", 0f);
        }
        private void _HideInstantly()
        {
            foreach (var g in target)
                g.alpha = 0f;
            Invoke("DoOnFadeOutEnded", 0f);
        }

        private void _Fade(CanvasGroup g, float duration, bool reverse)
        {
            J.Instance.JLerp(x => g.alpha = x, duration: duration, repeat: 1, type: CurveType.Linear, reverse: reverse, callingScript: this);
        }






    }

}