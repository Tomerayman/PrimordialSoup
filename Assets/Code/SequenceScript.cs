using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SequenceScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    private SoundSynchronizer.SoundData nestedSoundData;
    [FMODUnity.EventRef]
    public string defaultSound;
    public bool isNestingChord = false;
    public List<string> nestedChordSounds;
    public int bulbsNum;
    public bool isPlaying;
    public float litRatio;
    private float randomRatio;
    public List<MeshRenderer> bulbs;
    [SerializeField] private Transform tanksContainer;
    private List<Material> pulses;
    private static readonly int pulseID = Shader.PropertyToID("_Alpha");
    private List<int> _offBulbs;
    private List<int> _litBulbs;
    private bool _ratioChanged;
    private UIController _uiController;
    private LibObjectScript libScript;
    [SerializeField] private GameObject nestedObject;
    [SerializeField] private float ringRotationSpeed;
    [SerializeField] private Transform _ringTransform;

    void Start()
    {
        libScript = GetComponent<LibObjectScript>();
         _ratioChanged = false;
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        randomRatio = 0;
        nestedSoundData.sound = defaultSound;
        nestedSoundData.volume = 0.75f;
        // nestedSoundData.effectNames = new List<string>(new[] {"Send to Chorus", "Send to Delay", "Send to Tremolo"});
        nestedSoundData.effectVals = new List<float>(new[] {0f, 0f, 0f});
        ResetBulbs();
        RandomizeBulbs();
        InitPulses();
        StartCoroutine(SequenceSoundEmit());
        _uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
    }

    private void ResetBulbs()
    {
        _litBulbs = new List<int>();
        _offBulbs = new List<int>();
        for (int i = 0; i < bulbsNum; i++)
        {
            _offBulbs.Add(i);
        }
    }
    
    private void RandomizeBulbs()
    {
        int bulbsToLight = Mathf.RoundToInt(litRatio * bulbsNum);

        for (int i = 0; i < bulbsToLight; i++)
        {
            int r = Random.Range(0, _offBulbs.Count);
            _litBulbs.Add(_offBulbs[r]);
            _offBulbs.RemoveAt(r);
        }
        for (int i = 0; i < _litBulbs.Count; i++)
        {
            bulbs[_litBulbs[i]].enabled = true;
        }
        for (int i = 0; i < _offBulbs.Count; i++)
        {
            bulbs[_offBulbs[i]].enabled = false;
        }
        
    }

    private void InitPulses()
    {
        pulses = new List<Material>();
        for (int i = 1; i <= bulbsNum; i++)
        {
            Material mtl = tanksContainer.Find("Tank" + i.ToString()).GetChild(0).GetComponent<MeshRenderer>().material;
            mtl.SetFloat(pulseID, 0);
            pulses.Add(mtl);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        _ringTransform.Rotate(Vector3.up * (ringRotationSpeed * Time.deltaTime));
        if (_ratioChanged && !Input.GetMouseButton(0))
        {
            _ratioChanged = false;
            ChangeBulbs();
        }
    }

    public void PlaySound(bool effect)
    {
        // _currDispAmount = 0.3f;
        // SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        // soundData.sound = sound;
        // soundData.volume = libScript.GetVolumeFromScale();
        // more sound definitions (effects..)
        
        // soundManager.sounds.Add(soundData);
        if (isNestingChord)
        {
            nestedSoundData.sound = nestedChordSounds[Random.Range(0, 4)];
        }
        nestedSoundData.volume = libScript.GetVolumeFromScale();
        soundManager.SimplePlay(nestedSoundData);
    }
    
    IEnumerator SequenceSoundEmit()
    {
        int i = 0;
        while(true)
        {
            if (isPlaying)
            {
                if (_litBulbs.Contains(i))
                {
                    StartCoroutine(PulseCycle(pulses[i]));
                    PlaySound(false);
                }

                if (i < bulbsNum - 1)
                {
                    i++;
                }
                else
                {
                    i = 0;
                    ChangeByRandomRatio();
                }
                yield return new WaitForSeconds(6f / bulbsNum);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void ChangeRatio(float newValue)
    {
        litRatio = newValue;
        _ratioChanged = true;
    }

    public void ChangeRandom(float newValue)
    {
        randomRatio = newValue;
    }

    public void ChangeBulbs()
    {
        int neededBulbs = Mathf.RoundToInt(litRatio * bulbsNum);
        int bulbsToLight = neededBulbs - _litBulbs.Count;
        if (bulbsToLight > 0) // need to add bulbs
        {
            for (int i = 0; i < bulbsToLight; i++)
            {
                int r = Random.Range(0, _offBulbs.Count);
                _litBulbs.Add(_offBulbs[r]);
                bulbs[_offBulbs[r]].enabled = true;
                _offBulbs.RemoveAt(r);
            }
        }
        else if (bulbsToLight < 0) // need to turn off bulbs
        {
            for (int i = 0; i < -bulbsToLight; i++)
            {
                int r = Random.Range(0, _litBulbs.Count);
                _offBulbs.Add(_litBulbs[r]);
                bulbs[_litBulbs[r]].enabled = false;
                _litBulbs.RemoveAt(r);
            }
        }
    }

    private void ChangeByRandomRatio()
    {
        int bulbsToMove = Mathf.RoundToInt(randomRatio * _litBulbs.Count);
        for (int i = 0; i < bulbsToMove; i++)
        {
            if (_litBulbs.Count > 0 && _offBulbs.Count > 0)
            {
                int litIdx = Random.Range(0, _litBulbs.Count);
                int offIdx = Random.Range(0, _offBulbs.Count);
                _litBulbs.Add(_offBulbs[offIdx]);
                bulbs[_offBulbs[offIdx]].enabled = true;
                _offBulbs.RemoveAt(offIdx);
                _offBulbs.Add(_litBulbs[litIdx]);
                bulbs[_litBulbs[litIdx]].enabled = false;
                _litBulbs.RemoveAt(litIdx);
            }
        }
    }

    public void SetSequencerUI()
    {
        _uiController.HideAllElements();
        _uiController.objectSlider1.gameObject.SetActive(true);
        _uiController.objectSlider1.value = litRatio;
        _uiController.objectSlider1.onValueChanged.AddListener(ChangeRatio);
        _uiController.objectSlider2.gameObject.SetActive(true);
        _uiController.objectSlider2.value = randomRatio;
        _uiController.objectSlider2.onValueChanged.AddListener(ChangeRandom);
        // _uiController.onOffButton.gameObject.SetActive(true);
        // _uiController.onOffButton.onClick.AddListener(SetPlaying);
        // _uiController.onOffButton.image.sprite =
        //     (isPlaying) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Sample") || other.CompareTag("Chord")) &&
            !GetComponent<LibObjectScript>().getIsDragged())
            // && other.GetComponent<LibObjectScript>().getIsDragged())
        {
            StartCoroutine(WaitForNestedRelease(other.gameObject));
        }
    }
    
    IEnumerator WaitForNestedRelease(GameObject sampleObject)
    {
        yield return new WaitUntil(() => !Input.GetMouseButton(0));
        if (sampleObject != null &&
            Vector3.Distance(sampleObject.transform.position, transform.position) <
            GetComponent<SphereCollider>().radius * transform.localScale.x)
        {
            nestObject(sampleObject);
        }
    }
    
    private void nestObject(GameObject ObjectToNest)
    {
        if (!isPlaying)
        {
            isPlaying = true;
        }
        Destroy(nestedObject);
        Material newMtl;
        nestedObject = ObjectToNest;
        if (nestedObject.CompareTag("Sample"))
        {
            isNestingChord = false;
            SampleScript sampleScript = nestedObject.GetComponent<SampleScript>();
            sampleScript.isPlaying = false;
            nestedSoundData.sound = sampleScript.sound;
            nestedSoundData.effectNames = sampleScript.GetEffectNames();
            nestedSoundData.effectVals = sampleScript.effectStatus;
            nestedSoundData.volume = 0.75f;
            newMtl = nestedObject.GetComponent<Renderer>().material;
        }
        else
        {
            isNestingChord = true;
            ChordScript chordScript = nestedObject.GetComponent<ChordScript>();
            chordScript.isPlaying = false;
            nestedChordSounds = chordScript.sounds;
            nestedSoundData.effectNames = chordScript.GetEffectNames();
            nestedSoundData.effectVals = chordScript.effectStatus;
            nestedSoundData.volume = 0.75f;
            newMtl = chordScript._container.GetChild(0).GetComponent<Renderer>().material;

        }
        foreach (var bulb in bulbs)
        {
            bulb.material = newMtl;
        }
        nestedObject.transform.parent = transform;
        nestedObject.transform.localPosition = Vector3.zero;
        nestedObject.transform.localScale = nestedObject.transform.localScale / 2; 
        nestedObject.GetComponent<LibObjectScript>().isDraggable = false;
        nestedObject.GetComponent<Collider>().enabled = false;
    }

    public void SetPlaying()
    {
        isPlaying = !isPlaying;
        // _uiController.onOffButton.image.sprite =
        //     (isPlaying) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
    }

    public void DerenderOffBulbs()
    {
        foreach (var bulbIdx in _offBulbs)
        {
            bulbs[bulbIdx].enabled = false;
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