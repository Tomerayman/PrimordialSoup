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
    // public List<string> chords2;
    // [FMODUnity.EventRef]
    // public List<string> chords3;

    public List<string> sounds;
    public string chordLabel;
    private bool isRandom = false;
    public bool isPlaying = true;
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
        _mTransform = GetComponent<Transform>();
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
        
        sounds = chords1;
        _uiController.trinaryButtons[0].image.sprite = _uiController.onButtonSprite;
        // if (Math.Abs(setVal - 1) < 0.1f)
        // {
        //     sounds = chords1;
        //     _uiController.trinaryButtons[0].image.sprite = _uiController.onButtonSprite;
        // }
        // else if (Math.Abs(setVal - 2) < 0.1f)
        // {
        //     sounds = chords2;
        //     _uiController.trinaryButtons[1].image.sprite = _uiController.onButtonSprite;
        // }
        // else
        // {
        //     sounds = chords3;
        //     _uiController.trinaryButtons[2].image.sprite = _uiController.onButtonSprite;
        // }
    }
    
    public void PlaySound(bool effect)
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
        soundData.volume = libScript.GetVolumeFromScale();
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
        // _uiController.onOffButton.gameObject.SetActive(true);
        // _uiController.onOffButton.onClick.AddListener(SetPlaying);
        // _uiController.onOffButton.image.sprite =
        //     (isPlaying) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
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
}
