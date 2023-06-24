using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicAudioManager : MonoBehaviour
{
    public enum State {Calm, Fighting}
    public State state;

    public static DynamicAudioManager instance;

    public AudioMixer mixer;


    public float ChangeSpeed;
    public float ChangeSpeed_LowPass;

    [Range(10, 22000)]
    public float LowPass;

    [Range(0, 1)]
    public float BassVolume;
    [Range(0, 1)]
    public float DrumsVolume;
    [Range(0, 1)]
    public float PianoVolume;
    [Range(0, 1)]
    public float GuitarVolume;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mixer.SetFloat("BassVol", Mathf.Log10(BassVolume) * 20);
        mixer.SetFloat("DrumsVol", Mathf.Log10(DrumsVolume) * 20);
        mixer.SetFloat("PianoVol", Mathf.Log10(PianoVolume) * 20);
        mixer.SetFloat("GuitarVol", Mathf.Log10(GuitarVolume) * 20);
        mixer.SetFloat("LowPass", LowPass);

        if(state == State.Calm)
        {
            LowPass = Mathf.MoveTowards(LowPass, 300, ChangeSpeed_LowPass * Time.deltaTime);
            BassVolume = Mathf.MoveTowards(BassVolume, 1.0f, ChangeSpeed * Time.deltaTime);
            DrumsVolume = Mathf.MoveTowards(DrumsVolume, 0.0001f, ChangeSpeed * Time.deltaTime);
            PianoVolume = Mathf.MoveTowards(PianoVolume, 1.0f, ChangeSpeed * Time.deltaTime);
            GuitarVolume = Mathf.MoveTowards(GuitarVolume, 0.0001f, ChangeSpeed * Time.deltaTime);
        }
        else
        {
            LowPass = Mathf.MoveTowards(LowPass, 22000, ChangeSpeed_LowPass * Time.deltaTime);
            BassVolume = Mathf.MoveTowards(BassVolume, 1.0f, ChangeSpeed * Time.deltaTime);
            DrumsVolume = Mathf.MoveTowards(DrumsVolume, 1.0f, ChangeSpeed * Time.deltaTime);
            PianoVolume = Mathf.MoveTowards(PianoVolume, 1.0f, ChangeSpeed * Time.deltaTime);
            GuitarVolume = Mathf.MoveTowards(GuitarVolume, 1.0f, ChangeSpeed * Time.deltaTime);
        }
    }
}
