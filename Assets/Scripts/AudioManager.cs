using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    public GameObject AuthPlayer;
    public float mVolume = 0.0f;
    public float sVolume = 0.5f;

    private void Awake(){

        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

    }

    public void PlayMusic(string name){

        Sound s = Array.Find(musicSounds, x => x.name == name);

        if(s != null){

            musicSource.clip = s.clip;
            musicSource.volume = mVolume;
            musicSource.loop = true;
            musicSource.Play();

        }

    }

    public void PlaySFX(string name, GameObject call){

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s != null && Vector3.Distance(call.transform.position, AuthPlayer.transform.position) < 20){

            sfxSource.volume = sVolume;
            sfxSource.PlayOneShot(s.clip);

        }

    }

    public void SFXVolume(float i){

        sVolume = i;

    }

    public void MusicVolume(float i){

        mVolume = i;
        musicSource.volume = i;

    }

}
