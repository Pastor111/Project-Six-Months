using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    static List<AudioSource> speakers = new List<AudioSource>();

    public static bool isInitialized
    {
        get
        {
            return AudioParent != null;
        }
    }

    public static float MasterVolume = 1.0f;

    static GameObject AudioParent;

    public static void Init()
    {
        if (isInitialized)
            return;

        AudioParent = new GameObject("AudioManager");
        AudioParent.transform.position = Vector3.zero;
    }

    public static AudioSource PlaySound2D(AudioClip clip, bool loop = false, float volume = 1.0f, float pan = 0.0f, float pitch = 1.0f, AudioMixer mixer = null)
    {
        AudioSource source = new GameObject(clip.name + "_Source").AddComponent<AudioSource>();
        source.transform.parent = AudioParent.transform;
        source.transform.position = Vector3.zero;

        source.playOnAwake = false;
        source.volume = volume * MasterVolume;
        source.panStereo = pan;
        source.pitch = pitch;
        source.clip = clip;
        source.loop = loop;
        source.outputAudioMixerGroup = mixer.outputAudioMixerGroup;
        source.Play();
        if(!loop)
            Destroy(source.gameObject, source.clip.length);

        return source;
    }

    public static AudioSource PlaySound3D(AudioClip clip, bool loop = false, float volume = 1.0f, Vector3 position = new Vector3(), float minDistance = 0.0f, float MaxDistance = 10.0f, float pitch = 1.0f, AudioMixer mixer = null)
    {
        AudioSource source = new GameObject(clip.name + "_Source").AddComponent<AudioSource>();
        source.transform.parent = AudioParent.transform;
        source.transform.position = Vector3.zero;

        source.playOnAwake = false;
        source.volume = volume * MasterVolume;
        source.spatialBlend = 1.0f;
        source.transform.position = position;
        source.minDistance = minDistance;
        source.maxDistance = MaxDistance;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.pitch = pitch;
        source.clip = clip;
        source.loop = loop;
        source.outputAudioMixerGroup = mixer.outputAudioMixerGroup;
        source.Play();
        if (!loop)
            Destroy(source.gameObject, source.clip.length);

        return source;
    }

}
