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
    public bool isPlaying = true;
    public float minTimeBetweenNotes;
    public float maxTimeBetweenNotes;
    private UIController _uiController;
    private LibObjectScript libScript;
    private Material pulse;
    private static readonly int pulseID = Shader.PropertyToID("_Alpha");

    [SerializeField]
    private List<string> instrumentEffectNames; // list of parameter names of effects.
    public List<float> effectStatus; // list of bool values for effect status.
    public List<string> sampleModes;
    private int currModeIdx;
    private ParticleSystem nestedParticle;
    private GameObject nestedParticleGO;
    
    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        libScript = GetComponent<LibObjectScript>();
        pulse = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        pulse.SetFloat(pulseID, 0);
        effectStatus = new List<float>(new[] {0f, 0f, 0f});
        sampleModes = new List<string>(new[] {"none"}); 
        sampleModes.AddRange(instrumentEffectNames);
        currModeIdx = 0;
        StartCoroutine(SampleSoundEmit());
        _uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
    }

   
    public void PlaySound()
    {
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        soundData.sound = sound;
        soundData.effectNames = instrumentEffectNames;
        soundData.effectVals = effectStatus;
        soundData.volume = libScript.GetVolumeFromScale();
        if (currModeIdx == 0)
        {
            StartCoroutine(PulseCycle());
        }
        else
        {
            nestedParticle.Emit(1);
        }
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

    public List<string> GetEffectNames()
    {
        return instrumentEffectNames;
    }
   
    public void SetSampleUI()
    {
        _uiController.HideAllElements();
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

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Sample") || other.CompareTag("Chord"))
    //     {
    //         soundManager.EvaluteEvent(gameObject, other.gameObject);
    //     }
    // }

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

    public void SwitchPulse()
    {
        currModeIdx = (currModeIdx == 3) ? 0 : currModeIdx + 1;
        Destroy(nestedParticleGO);
        for (int i = 0; i < effectStatus.Count; i++)
        {
            effectStatus[i] = 0f;
        }
        if (currModeIdx == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            nestedParticleGO =
                Instantiate(Resources.Load(soundManager.parameterToObjectDict[sampleModes[currModeIdx]])) as GameObject;
            nestedParticle = nestedParticleGO.GetComponent<ParticleSystem>();
            effectStatus[currModeIdx - 1] = 1f;
            if (sampleModes[currModeIdx] == "Send to Delay") // echo filter
            {
                nestedParticle.GetComponent<ParticleSystemRenderer>().mesh = GetComponent<MeshFilter>().mesh;
            }
            Transform PSTransform = nestedParticle.transform;
            PSTransform.parent = transform;
            PSTransform.localScale = Vector3.one;
            PSTransform.localPosition = Vector3.zero;
        }
    }
    
    
}
    
    

