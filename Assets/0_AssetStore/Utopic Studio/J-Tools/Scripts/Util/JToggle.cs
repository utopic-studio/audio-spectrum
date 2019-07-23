using UnityEngine;

namespace J
{

	[AddComponentMenu("J/Util/JToggle")]
	public class JToggle : JBase
    {

		public void JToggleGameObject() {
			this.gameObject.SetActive( !this.gameObject.activeSelf );
		}
		public void JToggleCollider() {
			Collider col = GetComponent<Collider> ();
			if (col != null)
				col.enabled = !col.enabled;
		}
		public void JToggleColliderIsTrigger() {
			Collider col = GetComponent<Collider> ();
			if (col != null)
				col.isTrigger = !col.isTrigger;
		}
		public void JToggleRenderer() {
			Renderer rend = GetComponent<Renderer> ();
			if (rend != null)
				rend.enabled = !rend.enabled;
        }
        public void JToggleRigidbodyGravity() {
			Rigidbody rb = GetComponent<Rigidbody> ();
			if (rb != null)
				rb.useGravity = !rb.useGravity;
		}
		public void JToggleRigidbodyIsKinematic() {
			Rigidbody rb = GetComponent<Rigidbody> ();
			if (rb != null)
				rb.isKinematic = !rb.isKinematic;
		}
	}

}