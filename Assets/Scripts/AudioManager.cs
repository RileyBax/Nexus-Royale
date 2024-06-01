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
            musicSource.volume = 0.0f;
            musicSource.loop = true;
            musicSource.Play();

        }

    }

    public void PlaySFX(string name, GameObject call){

        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if(s != null && Vector3.Distance(call.transform.position, AuthPlayer.transform.position) < 20){

            sfxSource.PlayOneShot(s.clip);

        }

    }

}
