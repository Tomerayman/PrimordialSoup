using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private int _chordIdx = 0;
    private int _currSet = 1;
    private UIController _uiController;

    
    // Start is called before the first frame update
    void Start()
    {
        _mTransform = GetComponent<Transform>();
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        LibObjectScript libScript = GetComponent<LibObjectScript>();
        // libScript.slider1Action = SetChordGroup;
        // libScript.slider2Action = SetRandom;
        sounds = chords1;
        StartCoroutine(SampleSoundEmit());
        _uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRandom()
    {
        _uiController.binaryButton.image.sprite = (isRandom) ? _uiController.offButtonSprite : 
            _uiController.onButtonSprite;
        isRandom = !isRandom;
    }

    public void SetChordGroup(int setVal)
    {
        foreach (var b in _uiController.trinaryButtons)
        {
            b.image.sprite = _uiController.offButtonSprite;
        }
        if (Math.Abs(setVal - 1) < 0.1f)
        {
            sounds = chords1;
            _uiController.trinaryButtons[0].image.sprite = _uiController.onButtonSprite;
        }
        else if (Math.Abs(setVal - 2) < 0.1f)
        {
            sounds = chords2;
            _uiController.trinaryButtons[1].image.sprite = _uiController.onButtonSprite;
        }
        else
        {
            sounds = chords3;
            _uiController.trinaryButtons[2].image.sprite = _uiController.onButtonSprite;
        }
    }
    
    public void PlaySound(bool effect)
    {
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        if (isRandom)
        {
            soundData.sound = sounds[Random.Range(0, 4)];    
        }
        else
        {
            soundData.sound = sounds[_chordIdx];
            _chordIdx = (_chordIdx == 4) ? 0 : _chordIdx + 1;
        }
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
    
    public void SetChordUI()
    {
        _uiController.HideAllElements();
        _uiController.binaryButton.gameObject.SetActive(true);
        _uiController.binaryButton.onClick.AddListener(SetRandom);
        _uiController.binaryButton.image.sprite =
            (isRandom) ? _uiController.onButtonSprite : _uiController.offButtonSprite;

        for (int i = 0; i < _uiController.trinaryButtons.Count; i++)
        {
            _uiController.trinaryButtons[i].gameObject.SetActive(true);
            _uiController.trinaryButtons[i].image.sprite =
                (i + 1 == _currSet) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
        }
        _uiController.trinaryButtons[0].onClick.AddListener(delegate { SetChordGroup(1); });
        _uiController.trinaryButtons[1].onClick.AddListener(delegate { SetChordGroup(2); });
        _uiController.trinaryButtons[2].onClick.AddListener(delegate { SetChordGroup(3); });
    }
    
    
}
