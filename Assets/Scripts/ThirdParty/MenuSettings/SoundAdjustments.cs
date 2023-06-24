using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MainMenuUtils
{
    public class SoundAdjustments : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float GeneralVolume;
        [Range(0f, 1f)]
        public float AmbientSound;
        [Range(0f, 1f)]
        public float VFXSounds;

        public AudioMixer AmBientSoundMixer;
        public AudioMixer VFXSoundMixer;


        // Start is called before the first frame update
        void Start()
        {
            //if (!CheckForObjectsEveryFrame)
            //{
            //    sources = GameObject.FindObjectsOfType<AudioSource>();
            //}
        }

        // Update is called once per frame
        void Update()
        {
            //if (CheckForObjectsEveryFrame)
            //{
            //    sources = GameObject.FindObjectsOfType<AudioSource>();
            //}

            float ambient = AmbientSound * GeneralVolume;
            float vfx = VFXSounds * GeneralVolume;

            AmBientSoundMixer.SetFloat("Volume", Mathf.Log10(ambient) * 20);
            VFXSoundMixer.SetFloat("Volume", Mathf.Log10(vfx) * 20);

            //for (int i = 0; i < sources.Length; i++)
            //{
            //    if (sources[i].gameObject.CompareTag(AmBientSoundTag))
            //        sources[i].volume = ambient;

            //    if (sources[i].gameObject.CompareTag(VFXSoundTag))
            //        sources[i].volume = vfx;
            //}
        }
    }
}
