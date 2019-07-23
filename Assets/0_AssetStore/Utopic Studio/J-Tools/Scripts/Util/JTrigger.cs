using UnityEngine;

namespace J
{

	[AddComponentMenu("J/Util/JTrigger")]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Collider))]
    public class JTrigger : JBase
    {

        [Tooltip("Dejar vacío para activarse con cualquier otro objeto")]
		[SerializeField]	string tagCondition = "";
		[SerializeField]	bool callEnterOnce = true;
		[SerializeField]	bool callExitOnce = true;
		[SerializeField]	UnityEngine.Events.UnityEvent onEnter;
		[SerializeField]	UnityEngine.Events.UnityEvent onExit;

		protected Collider coll;
		protected bool m_enterEnabled = true, m_exitEnabled = true;


        private void Start () {
            this.Reset();
		}

        private void OnValidate()
        {
            tagCondition = tagCondition.Trim();
        }

        private void Reset()
        {
            Renderer rend = GetComponent<Renderer>();
            Collider col = GetComponent<Collider>();
            if (rend)
                rend.enabled = false;
            if (col)
                col.isTrigger = true;
        }


        private void OnTriggerEnter(Collider other) {
			if (!isTagValid (other.tag))
				return;
			if (m_enterEnabled) {
				onEnter.Invoke ();
			}
			if (callEnterOnce)
				m_enterEnabled = false;
		}
		private void OnTriggerExit(Collider other) {
			if (!isTagValid (other.tag))
				return;
			if (m_exitEnabled) {
				onExit.Invoke ();
			}
			if (callExitOnce)
				m_exitEnabled = false;
		}
		private bool isTagValid(string tag) {
			if (tagCondition == "")
				return true;
			return tag == tagCondition;
		}
        
	}

}