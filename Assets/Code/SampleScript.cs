using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    [FMODUnity.EventRef]
    public string sound;
    private MeshRenderer _meshRenderer;
    private float _minDispAmount;
    private float _currDispAmount;
    private static readonly int amountID = Shader.PropertyToID("_Amount");

    public bool isPlaying = true;
    public float minTimeBetweenNotes;
    public float maxTimeBetweenNotes;
    private int soundStartTime;
    private int soundEndTime;
    private int soundLengthInMilliSec;
    private UIController _uiController;


    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _minDispAmount = _meshRenderer.material.GetFloat(amountID);
        LibObjectScript libScript = GetComponent<LibObjectScript>();
        FMOD.Studio.EventInstance tempEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
        FMOD.Studio.EventDescription description;
        tempEvent.getDescription(out description);
        description.getLength(out soundLengthInMilliSec);
        soundStartTime = 0;
        soundEndTime = soundLengthInMilliSec;
        StartCoroutine(SampleSoundEmit());
        _uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        _currDispAmount = Mathf.Lerp(_currDispAmount, _minDispAmount, Time.deltaTime);
        _meshRenderer.material.SetFloat(amountID, _currDispAmount);
    }
    
    public void PlaySound(bool effect)
    {
        _currDispAmount = 0.3f;
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        
        soundData.sound = sound;
        soundData.customPlayback = (soundStartTime > 3 || soundEndTime < soundLengthInMilliSec - 3);
        // soundData.startTime = soundStartTime;
        // soundData.endTime = soundEndTime;
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

    public void SetSoundStart(float newValue)
    {
        soundStartTime = Mathf.RoundToInt(newValue * soundLengthInMilliSec);
        
        if (soundStartTime > soundEndTime)
        {
            soundStartTime = soundEndTime;
        }
    }
    
    public void SetSoundEnd(float newValue)
    {
        soundEndTime = Mathf.RoundToInt(newValue * soundLengthInMilliSec);
        if (soundStartTime > soundEndTime)
        {
            soundEndTime = soundStartTime;
        }
    }
    
    public void SetSampleUI()
    {
        _uiController.HideAllElements();
        _uiController.objectSlider1.gameObject.SetActive(true);
        _uiController.objectSlider1.value = (float) soundStartTime / soundLengthInMilliSec;
        _uiController.objectSlider1.onValueChanged.AddListener(SetSoundStart);
        _uiController.objectSlider2.gameObject.SetActive(true);
        _uiController.objectSlider2.value = (float) soundEndTime / soundLengthInMilliSec;
        _uiController.objectSlider2.onValueChanged.AddListener(SetSoundEnd);
        _uiController.onOffButton.gameObject.SetActive(true);
        _uiController.onOffButton.onClick.AddListener(SetPlaying);
        _uiController.onOffButton.image.sprite =
            (isPlaying) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
    }

    public void SetPlaying()
    {
        isPlaying = !isPlaying;
        _uiController.onOffButton.image.sprite =
            (isPlaying) ? _uiController.onButtonSprite : _uiController.offButtonSprite;
    }
    
    // private IEnumerator soundEndEnforcer(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     soundMaker.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    // }
}
