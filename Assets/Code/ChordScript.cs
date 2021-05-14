using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChordScript : MonoBehaviour
{
    private Transform _mTransform;
    public SoundSynchronizer soundManager;
    [FMODUnity.EventRef]
    public List<string> chords1;
    [FMODUnity.EventRef]
    public List<string> chords2;
    [FMODUnity.EventRef]
    public List<string> chords3;

    private List<string> sounds;
    private bool isRandom = false;
    public bool isPlaying = true;
    public float minTimeBetweenNotes;
    public float maxTimeBetweenNotes;
    
    // Start is called before the first frame update
    void Start()
    {
        _mTransform = GetComponent<Transform>();
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        LibObjectScript libScript = GetComponent<LibObjectScript>();
        libScript.slider1Action = SetChordGroup;
        libScript.slider2Action = SetRandom;
        sounds = chords1;
        StartCoroutine(SampleSoundEmit());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRandom(float newValue)
    {
        
    }

    public void SetChordGroup(float newValue)
    {
        if (Math.Abs(newValue - 1) < 0.1f)
        {
            sounds = chords1;
        }
        else if (Math.Abs(newValue - 2) < 0.1f)
        {
            sounds = chords2;
        }
        else
        {
            sounds = chords3;
        }
    }
    
    public void PlaySound(bool effect)
    {
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        soundData.sound = sounds[Random.Range(0, 4)];
        soundData.customPlayback = false;
        // more sound definitions (effects..)
        soundManager.sounds.Add(soundData);
    }
    
    IEnumerator SampleSoundEmit()
    {
        float delay;
        while(true)
        {
            if (isPlaying)
            {
                PlaySound(false);
            }
            delay = Random.Range(minTimeBetweenNotes, maxTimeBetweenNotes);
            yield return new WaitForSeconds(delay);
        }
    }
}
