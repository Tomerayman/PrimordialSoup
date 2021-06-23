using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChordScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    [FMODUnity.EventRef]
    public List<string> chords1;
    [FMODUnity.EventRef]
    // public List<string> chords2;
    // [FMODUnity.EventRef]
    // public List<string> chords3;

    public List<string> sounds;
    public string chordLabel;
    private bool isRandom = false;
    public bool isPlaying = true;
    [SerializeField] private List<string> instrumentEffects;
    public List<float> effectStatus; // list of bool values for effect status.

    public float minTimeBetweenNotes;
    public float maxTimeBetweenNotes;
    private int _chordIdx = 0;
    private int _currSet = 1;
    private UIController _uiController;
    private LibObjectScript libScript;
    [SerializeField] private Transform _container;
    private List<Material> pulses;
    private static readonly int pulseID = Shader.PropertyToID("_Alpha");

    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        libScript = GetComponent<LibObjectScript>();
        // libScript.slider1Action = SetChordGroup;
        // libScript.slider2Action = SetRandom;
        sounds = chords1;
        pulses = new List<Material>();
        foreach (Transform child in _container)
        {
            Material mtl = child.GetChild(0).GetComponent<MeshRenderer>().material;
            mtl.SetFloat(pulseID, 0);
            pulses.Add(mtl);
        }
        effectStatus = new List<float>(new[] {0f, 0f, 0f});
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

   
    public void PlaySound()
    {
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        if (isRandom)
        {
            int idx = Random.Range(0, 4);
            soundData.sound = sounds[idx];
            StartCoroutine(PulseCycle(pulses[idx]));
        }
        else
        {
            soundData.sound = sounds[_chordIdx];
            StartCoroutine(PulseCycle(pulses[_chordIdx]));
            _chordIdx = (_chordIdx == 4) ? 0 : _chordIdx + 1;
        }
        soundData.effectNames = instrumentEffects;
        soundData.effectVals = effectStatus;
        soundData.volume = libScript.GetVolumeFromScale();
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
                PlaySound();
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

        // for (int i = 0; i < _uiController.trinaryButtons.Count; i++)
        // {
        //     _uiController.trinaryButtons[i].gameObject.SetActive(true);
        //     _uiController.trinaryButtons[i].image.sprite =
        //         (i + 1 == _currSet) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
        // }
        // _uiController.trinaryButtons[0].onClick.AddListener(delegate { SetChordGroup(1); });
        // _uiController.trinaryButtons[1].onClick.AddListener(delegate { SetChordGroup(2); });
        // _uiController.trinaryButtons[2].onClick.AddListener(delegate { SetChordGroup(3); });
        for (int i = 0; i < 3; i++)
        {
            _uiController.effectButtons[i].gameObject.SetActive(true);
            var i1 = i;
            _uiController.effectButtons[i].onClick.AddListener(delegate { SetEffect(i1); });
            _uiController.effectButtons[i].image.sprite = (Math.Abs(effectStatus[i] - 1f) < 0.1f)
                ? _uiController.effectSpritesDict[instrumentEffects[i]].Item1
                : _uiController.effectSpritesDict[instrumentEffects[i]].Item2;
        }
    }

    public void SetPlaying()
    {
        isPlaying = !isPlaying;
        // _uiController.onOffButton.image.sprite =
        //     (isPlaying) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sample") || other.CompareTag("Chord"))
        {
            soundManager.EvaluteEvent(gameObject, other.gameObject);
        }
    }
    
    IEnumerator PulseCycle(Material pulse)
    {
        float duration = 1f;
        float time = 0;
        while (time < duration / 5) 
        {
            pulse.SetFloat(pulseID, 5* time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds((3 * duration) / 5);
        time = 0;
        while (time < duration / 5)
        {
            pulse.SetFloat(pulseID, 1 - 5* time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        pulse.SetFloat(pulseID, 0);
    }
    
    public void SetEffect(int effectNum)
    {
        if (effectStatus[effectNum] < 0.5f) // effect is off
        {
            effectStatus[effectNum] = 1;
            // switch button sprite to "on":
            _uiController.effectButtons[effectNum].image.sprite =
                _uiController.effectSpritesDict[instrumentEffects[effectNum]].Item1;
        }
        else
        {
            effectStatus[effectNum] = 0;
            _uiController.effectButtons[effectNum].image.sprite =
                _uiController.effectSpritesDict[instrumentEffects[effectNum]].Item2;
        }
    }
}
