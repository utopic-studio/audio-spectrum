using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class MySpriteSequence : MonoBehaviour {

	public bool startsVisible;
	public float transitionTime;
	public float startAfter;
	[SerializeField]
	Sprite[] spritesArray;
	[TooltipAttribute("visible duration of each of the images")]
	public float[] durations;

	private UnityEngine.UI.Image img;
	private bool fadeIn, fadeOut;
	private float lerpVar;
	private Color originalColor;
	private Color transparentColor;

	// Use this for initialization
	void Start () {
		img = GetComponent<UnityEngine.UI.Image> ();
		originalColor = img.color;
		transparentColor = new Color (originalColor.r, originalColor.g, originalColor.b, 0f);

		img.color = startsVisible ? originalColor : transparentColor;

		print (spritesArray [0]);
		print (spritesArray [1]);

		StartCoroutine (WaitBeforeMainLoop (startAfter));
	}
	IEnumerator WaitBeforeMainLoop(float n) {
		yield return new WaitForSeconds (n);
		if (spritesArray.Length > 1)
			StartCoroutine (SpriteSequenceLoop ());
	}

	IEnumerator SpriteSequenceLoop () {
		int index = 0;

		if (!startsVisible) {
			FadeIn ();
			yield return new WaitForSeconds (transitionTime);
		}

		while (this.enabled) {
			yield return new WaitForSeconds (durations [index]);
			FadeOut ();
			yield return new WaitForSeconds (transitionTime);

			index++;
			index = index == spritesArray.Length ? 0 : index;

			img.sprite = spritesArray [index];

			FadeIn ();
			yield return new WaitForSeconds (transitionTime);
		}
	}


	void FadeIn() {
		fadeIn = true;
		fadeOut = false;
		lerpVar = 0f;
	}
	void FadeOut() {
		fadeOut = true;
		fadeIn = false;
		lerpVar = 0f;
	}

	void Update () {
		if (fadeIn) {
			//print ("fadeIn");
			lerpVar += Time.deltaTime / transitionTime;
			img.color = Color.Lerp (transparentColor, originalColor, lerpVar);
		} else if (fadeOut) {
			//print ("fadeOUtOUTTTT");
			lerpVar += Time.deltaTime / transitionTime;
			img.color = Color.Lerp (originalColor, transparentColor, lerpVar);
		}
	}


}