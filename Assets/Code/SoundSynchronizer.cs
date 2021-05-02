using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSynchronizer : MonoBehaviour
{
    FMOD.Studio.EventInstance soundMaker;

    // Start is called before the first frame update
    public List<(string, bool)> sounds;

    void Start()
    {
        sounds = new List<(string, bool)>();
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
                soundMaker = FMODUnity.RuntimeManager.CreateInstance(sounds[i].Item1);
                // soundMaker.setVolume(SettingsMenu.golbalVolume);
                float effectVal = (sounds[i].Item2) ? 1f : 0f;
                soundMaker.setParameterByName("Effect1", effectVal);
                soundMaker.start();
                sounds.Remove(sounds[i]);
            }
            yield return new WaitForSeconds(0.125f);
        }
    }
    //void soundSync()
    //{
    //    foreach (string sound in sounds)
    //    {
    //        soundMaker = FMODUnity.RuntimeManager.CreateInstance(sound);
    //        soundMaker.start();
    //        sounds.Remove(sound);
    //    }
    //}
}
