using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[AddComponentMenu("J/3D/JFadeLight")]
public class JFadeLight : MonoBehaviour
{
    public bool Fade;
    public Light FadeLight;
    public AnimationCurve Intensity;
    public float Duration;
    float InitIntensity;

    // Start is called before the first frame update
    void Start()
    {
        FadeLight.GetComponent<Light>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Fade)
        {
            Duration = Mathf.Clamp(Duration, 0, 200);
            Duration +=Time.deltaTime;
            FadeLight.intensity = Intensity.Evaluate(Duration);
        }
    }
    public void OnFade()
    {
        if (Fade)
        {
            Fade = false;
        }
        else
        {
            Fade = true;
        }
    }
}
