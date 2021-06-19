using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundSynchronizer : MonoBehaviour
{
    public struct SoundData
    {
        public string sound;
        public float volume;
        public float effect1;
        public float effect2;
        public float effect3;
        public bool customPlayback;
    }
    
    FMOD.Studio.EventInstance soundMaker;
    public List<SoundData> sounds;
    // Start is called before the first frame update

    private void Awake()
    {
        sounds = new List<SoundData>();
    }

    void Start()
    {
        StartCoroutine(syncEmitter());
        //InvokeRepeating("soundSync", 0, 1 / 16);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SimplePlay(SoundData soundData)
    {
        soundMaker = FMODUnity.RuntimeManager.CreateInstance(soundData.sound);
        // soundMaker.setVolume(SettingsMenu.golbalVolume);
        // float effectVal = (sounds[i].Item2) ? 1f : 0f;
        // soundMaker.setParameterByName("Effect1", effectVal);
        soundMaker.setVolume(soundData.volume);
        soundMaker.start();
    }
    
    IEnumerator syncEmitter()
    {
        while (true)
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                SimplePlay(sounds[i]);
                sounds.Remove(sounds[i]);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    

}
