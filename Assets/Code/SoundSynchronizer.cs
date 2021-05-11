using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SoundSynchronizer : MonoBehaviour
{
    public struct SoundData
    {
        public string sound;
        public float effect1;
        public float effect2;
        public float effect3;
        public bool customPlayback;
        public int startTime;
        public int endTime;
    }
    
    FMOD.Studio.EventInstance soundMaker;

    // Start is called before the first frame update
    public List<SoundData> sounds;

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


    IEnumerator syncEmitter()
    {
        while (true)
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                soundMaker = FMODUnity.RuntimeManager.CreateInstance(sounds[i].sound);
                if (sounds[i].customPlayback)
                {
                    soundMaker.setTimelinePosition(sounds[i].startTime);
                    float delayInSeconds = (sounds[i].endTime - sounds[i].startTime) / 1000f;
                    StartCoroutine(soundEndEnforcer(delayInSeconds));
                }
                // soundMaker.setVolume(SettingsMenu.golbalVolume);
                // float effectVal = (sounds[i].Item2) ? 1f : 0f;
                // soundMaker.setParameterByName("Effect1", effectVal);
                soundMaker.start();
                sounds.Remove(sounds[i]);
            }
            yield return new WaitForSeconds(0.125f);
        }
    }
    
    private IEnumerator soundEndEnforcer(float delay)
    {
        yield return new WaitForSeconds(delay);
        soundMaker.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
