using System;
using UnityEngine;

namespace J
{

    [AddComponentMenu("J/Util/JMusic")]
    public class JMusic : JBase
    {
        
        [SerializeField] AudioClip[] audioTracks;
        [SerializeField] bool overrideAudioSettings = false;
        [SerializeField] bool loop = true;
        [SerializeField] float beginDelay = 0f;
        [SerializeField] float fadeIn = 1f;
        [Range(0f, 1f)]
        [SerializeField] float volume = 0.5f;

        private void Start()
        {
            Invoke("_Start", beginDelay);
        }
        private void _Start()
        {
            //MyManager.Instance.audioManager.PlayMusic(levelMusic, volume, fadeIn);
            Prev();
        }



        public void Prev()
        {
            throw new NotImplementedException();
        }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void PausePlay()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void FadeOut()
        {
            throw new NotImplementedException();
        }

        public void FadeIn()
        {
            throw new NotImplementedException();
        }
    }
}