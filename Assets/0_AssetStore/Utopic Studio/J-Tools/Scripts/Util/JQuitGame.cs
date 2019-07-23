using UnityEngine;

namespace J
{
	[AddComponentMenu("J/Util/JQuitGame")]
	public class JQuitGame : JBase
    {

		[SerializeField]	bool worksInEditor = false;

		void Update () {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Quit ();
			}
		}

		public void Quit () {
			Application.Quit ();
#if UNITY_EDITOR
            if (worksInEditor) {
				UnityEditor.EditorApplication.isPlaying = false;
			}
#endif
        }
	}

}