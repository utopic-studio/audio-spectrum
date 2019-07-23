using UnityEngine;

namespace J
{
	[AddComponentMenu("J/Util/JDontDestroy")]
	public class JDontDestroy : JBase
    {
		void Awake () {
			DontDestroyOnLoad (this.gameObject);
		}
	}
}