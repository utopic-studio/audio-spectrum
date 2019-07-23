using UnityEngine;

namespace J
{
	
	[AddComponentMenu("J/Util/JSceneLoad")]
	public class JSceneLoad : JBase
    {

        [Tooltip("Escena debe estar en Build Settings")]
		[SerializeField]    string sceneName;
		[SerializeField]	float delay = 0f;

        public void LoadScene () {
			Invoke ("_LoadScene", delay);
		}
		private void _LoadScene () {
			sceneName = sceneName.Trim ();
			if (sceneName != null && sceneName != "") {

                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            }
		}


    }

}