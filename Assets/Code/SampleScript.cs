using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    private Transform _mTransform;
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

    // Start is called before the first frame update
    void Start()
    {
        _mTransform = GetComponent<Transform>();
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _minDispAmount = _meshRenderer.material.GetFloat(amountID);
        LibObjectScript libScript = GetComponent<LibObjectScript>();
        libScript.slider1Action = SetSoundStart;
        libScript.slider2Action = SetSoundEnd;
        
        FMOD.Studio.EventInstance tempEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
        FMOD.Studio.EventDescription description;
        tempEvent.getDescription(out description);
        description.getLength(out soundLengthInMilliSec);
        soundStartTime = 0;
        soundEndTime = soundLengthInMilliSec;
        StartCoroutine(SampleSoundEmit());
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
        soundData.startTime = soundStartTime;
        soundData.endTime = soundEndTime;
        // Debug.Log("custom: " + soundData.customPlayback + ", start: " + soundData.startTime + ", end: " +
        //       soundData.endTime);
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
}
