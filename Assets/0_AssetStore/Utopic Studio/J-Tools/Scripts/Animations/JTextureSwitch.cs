using UnityEngine;

namespace J
{

    [AddComponentMenu("J/Animations/JTextureSwitch")]
    public class JTextureSwitch : JBase
    {
        //Claudio Inostroza Bull shit Script

        public Renderer MeshChangeTexture;//
        Texture textureChange;
        Texture originalTexture;
        public Texture SwichTexture;
        public Texture SwichTextureBlink;
        public bool switched = true;
        float TimeOfBlink;
        public float TimeOfBlinkMax = 50f;
        public float DurationOfBlink = 0.3f;//Duration of blink
        public float RandomBlinkFactor = 1.5f;//Randomize the blink

        // Start is called before the first frame update
        void Start()
        {
            textureChange = MeshChangeTexture.GetComponent<Renderer>().material.mainTexture;
            originalTexture = textureChange;
            //MeshChangeTexture.material.mainTexture = SwichTextureBlink;
        }

        // Update is called once per frame
        void Update()
        {
            TimeOfBlink = Mathf.Clamp(TimeOfBlink, 0, TimeOfBlinkMax);
            TimeOfBlink++;

            if (switched)
            {
                if (TimeOfBlink >= Random.Range(TimeOfBlinkMax, TimeOfBlinkMax * RandomBlinkFactor))
                {
                    MeshChangeTexture.material.mainTexture = SwichTextureBlink;
                    Invoke("TextureReset", DurationOfBlink);
                    TimeOfBlink = 0;
                }

            }
            else
            {
                MeshChangeTexture.material.mainTexture = originalTexture;
            }
        }
        void TextureReset()
        {
            MeshChangeTexture.material.mainTexture = SwichTexture;
        }

    }
}