using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SampleScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    [FMODUnity.EventRef]
    public string sound;
    public string sampleLabel;
    private MeshRenderer _meshRenderer;
    // private float _minDispAmount;
    // private float _currDispAmount;
    // private static readonly int amountID = Shader.PropertyToID("_Amount");

    public bool isPlaying = true;
    public float minTimeBetweenNotes;
    public float maxTimeBetweenNotes;
    private int soundStartTime;
    private int soundEndTime;
    private int soundLengthInMilliSec;
    private UIController _uiController;
    private LibObjectScript libScript;
    private Material pulse;
    private static readonly int pulseID = Shader.PropertyToID("_Alpha");

    [SerializeField]
    private List<string> instrumentEffectNames; // list of parameter names of effects.
    public List<float> effectStatus; // list of bool values for effect status.
    
    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        // _minDispAmount = _meshRenderer.material.GetFloat(amountID);
        libScript = GetComponent<LibObjectScript>();
        FMOD.Studio.EventInstance tempEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
        FMOD.Studio.EventDescription description;
        tempEvent.getDescription(out description);
        description.getLength(out soundLengthInMilliSec);
        soundStartTime = 0;
        soundEndTime = soundLengthInMilliSec;
        pulse = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        pulse.SetFloat(pulseID, 0);
        effectStatus = new List<float>(new[] {0f, 0f, 0f});
        
        StartCoroutine(SampleSoundEmit());
        _uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     _currDispAmount = Mathf.Lerp(_currDispAmount, _minDispAmount, Time.deltaTime);
    //     _meshRenderer.material.SetFloat(amountID, _currDispAmount);
    // }
    
    public void PlaySound()
    {
        // _currDispAmount = 0.3f;
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        soundData.sound = sound;
        soundData.effectNames = instrumentEffectNames;
        soundData.effectVals = effectStatus;
        soundData.volume = libScript.GetVolumeFromScale();
        // soundData.startTime = soundStartTime;
        // soundData.endTime = soundEndTime;
        soundManager.sounds.Add(soundData);
        StartCoroutine(PulseCycle());
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

   
    public void SetSampleUI()
    {
        _uiController.HideAllElements();
        // _uiController.objectSlider1.gameObject.SetActive(true);
        // _uiController.objectSlider1.value = (float) soundStartTime / soundLengthInMilliSec;
        // _uiController.objectSlider1.onValueChanged.AddListener(SetSoundStart);
        // _uiController.objectSlider2.gameObject.SetActive(true);
        // _uiController.objectSlider2.value = (float) soundEndTime / soundLengthInMilliSec;
        // _uiController.objectSlider2.onValueChanged.AddListener(SetSoundEnd);
        for (int i = 0; i < 3; i++)
        {
            _uiController.effectButtons[i].gameObject.SetActive(true);
            var i1 = i;
            _uiController.effectButtons[i].onClick.AddListener(delegate { SetEffect(i1); });
            _uiController.effectButtons[i].image.sprite = (Math.Abs(effectStatus[i] - 1f) < 0.1f)
                ? _uiController.effectSpritesDict[instrumentEffectNames[i]].Item1
                : _uiController.effectSpritesDict[instrumentEffectNames[i]].Item2;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sample") || other.CompareTag("Chord"))
        {
            soundManager.EvaluteEvent(gameObject, other.gameObject);
        }
    }

    public void SetPlaying()
    {
        isPlaying = !isPlaying;
    }

    IEnumerator PulseCycle()
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
                _uiController.effectSpritesDict[instrumentEffectNames[effectNum]].Item1;
        }
        else
        {
            effectStatus[effectNum] = 0;
            _uiController.effectButtons[effectNum].image.sprite =
                _uiController.effectSpritesDict[instrumentEffectNames[effectNum]].Item2;
        }
    }
}
    
    

