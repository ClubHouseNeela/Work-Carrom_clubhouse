using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public float backgroundMusicDelay;
    public Sound[] sounds;

    private Sound isPlaying;
    private bool audioPaused = false;
    private bool mute = false;

    void Awake()
    {
        /*if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.outputAudioMixerGroup = s.audioMixerGroup;
            s.source.clip = s.clip;
            s.source.loop = s.loop;
        }
    }


    public bool Mute
    {
        get
        {
            return mute;
        }
        set
        {
            mute = value;
            if(value)
            {
                AudioPause();
            }
            else
            {
                ContinueAudio();
            }
        }
    }
    public void BackgroundMusicStart()
    {
        //Play("BackgroundMusic", 1f);
        StartCoroutine(BackgroundMusicDelayStart());
    }

    private IEnumerator BackgroundMusicDelayStart()
    {
        yield return new WaitForSecondsRealtime(backgroundMusicDelay);
        Play("BackgroundMusic", 1f);
    }

    // Plays the audio with the name in sound variable
    public void Play(string sound, float pitch = 1f, float delay = 0f, float volumeMultiplier = 1f)
    {
        if(Mute)
        {
            return;
        }

        isPlaying = Array.Find(sounds, item => item.name == sound);
        if (isPlaying == null)
        {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }
        if (!isPlaying.source.isPlaying)
        {
            isPlaying.audioMixerGroup.audioMixer.SetFloat("Pitch", ((1f / pitch)));
            isPlaying.source.volume = isPlaying.volume * volumeMultiplier;
            isPlaying.source.pitch = isPlaying.pitch * pitch;

            isPlaying.source.PlayDelayed(delay);
        }

    }

    public void AudioPause()
    {
        AudioListener.pause = true;
        audioPaused = true;
    }

    public void ContinueAudio()
    {
        if (audioPaused)
        {
            AudioListener.pause = false;
        }
    }

    public void Stop(string sound)
    {
        isPlaying = Array.Find(sounds, item => item.name == sound);
        if (isPlaying == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        isPlaying.source.Stop();
    }

    public void StopAllAudioExceptBackground()
    {
        foreach (Sound s in sounds)
        {
            if (s.name != "BackgroundMusic")
            {
                s.source.Stop();
            }
        }
    }

    //-----------------------------------------------------------------------------------------------------------------------
}
