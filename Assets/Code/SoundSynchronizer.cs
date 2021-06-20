using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SoundSynchronizer : MonoBehaviour
{
    public struct SoundData
    {
        public string sound;
        public float volume;
        public float effect1;
        public float effect2;
        public float effect3;
        public bool customPlayback;
    }

    public delegate void ComboFunction(GameObject a, GameObject b);
    public struct EventData
    {
        public HashSet<string> objectLabels;
        public ComboFunction comboFunction;

        public EventData(string[] labels, ComboFunction function)
        {
            objectLabels = new HashSet<string>(labels);
            comboFunction = function;
        }
    }
    
    
    FMOD.Studio.EventInstance soundMaker;
    public List<SoundData> sounds;
    public HashSet<(GameObject, GameObject)> currEvents;
    public HashSet<EventData> scriptedEvents;
    [SerializeField] private ParticleSystem comboExplosion;
    
    // Start is called before the first frame update

    private void Awake()
    {
        sounds = new List<SoundData>();
    }

    void Start()
    {
        StartCoroutine(syncEmitter());
        //InvokeRepeating("soundSync", 0, 1 / 16);
        currEvents = new HashSet<(GameObject, GameObject)>();
        scriptedEvents = new HashSet<EventData>(new[]
            { // scripted events defined here:
                new EventData(new string[]{"Si_01", "Si_02"}, PopcornJellyFishCombo)
            }
        );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SimplePlay(SoundData soundData)
    {
        soundMaker = FMODUnity.RuntimeManager.CreateInstance(soundData.sound);
        // soundMaker.setVolume(SettingsMenu.golbalVolume);
        // float effectVal = (sounds[i].Item2) ? 1f : 0f;
        // soundMaker.setParameterByName("Effect1", effectVal);
        soundMaker.setVolume(soundData.volume);
        soundMaker.start();
    }
    
    IEnumerator syncEmitter()
    {
        while (true)
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                SimplePlay(sounds[i]);
                sounds.Remove(sounds[i]);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    // events behaviour:

    public void EvaluteEvent(GameObject caller, GameObject other)
    {
        foreach ((GameObject, GameObject) ev in currEvents)
        {
            if ((ReferenceEquals(ev.Item1, caller) && ReferenceEquals(ev.Item2, other)) ||
                (ReferenceEquals(ev.Item2, caller) && ReferenceEquals(ev.Item1, other))) // event already exists and being examined
            {
                return;
            }
        }
        string callerLabel = caller.CompareTag("Sample") ? caller.GetComponent<SampleScript>().sampleLabel :
            caller.GetComponent<ChordScript>().chordLabel;
        string otherLabel = other.CompareTag("Sample") ? other.GetComponent<SampleScript>().sampleLabel :
            other.GetComponent<ChordScript>().chordLabel;
        currEvents.Add((caller, other));
        HashSet<string> currLabels = new HashSet<string>(new []{callerLabel, otherLabel});
        foreach (EventData scriptedEvent in scriptedEvents) // lookup current event against scripted ones
        {
            if (currLabels.SetEquals(scriptedEvent.objectLabels)) // found matching event
            {
                StartCoroutine(DelayEventRemoval((caller, other)));
                caller.GetComponent<LibObjectScript>().StopDrag();
                other.GetComponent<LibObjectScript>().StopDrag();
                scriptedEvent.comboFunction(caller, other); // initiate combo!
            }
        }
    }

    IEnumerator DelayEventRemoval((GameObject, GameObject) eventToRemove)
    {
        yield return new WaitForSeconds(5);
        currEvents.Remove(eventToRemove);
    }

    private void PopcornJellyFishCombo(GameObject popcorn, GameObject jellyfish)
    {
        StartCoroutine(ComboCR());
        IEnumerator ComboCR()
        {
            float time = 0;
            float duration = 2f;
            Vector3 jellyStartPos = jellyfish.transform.position;
            Vector3 popStartPos = popcorn.transform.position;
            Vector3 endPos = (popStartPos + jellyStartPos) * 0.5f;
            Vector3 jellyMidPos = (jellyStartPos + endPos) * 0.5f + Vector3.up;
            Vector3 popMidPos = (popStartPos + endPos) * 0.5f - Vector3.up;
            while (time < duration)
            {
                float st = LinearToS(time / duration, 2);
                jellyfish.transform.position =
                    BezierIntrp(jellyStartPos, jellyMidPos, endPos, st);
                popcorn.transform.position =
                    BezierIntrp(popStartPos, popMidPos, endPos, st);
                popcorn.transform.localScale = Vector3.Lerp(popcorn.transform.localScale, Vector3.zero, st);
                jellyfish.transform.localScale = Vector3.Lerp(jellyfish.transform.localScale, Vector3.zero, st);
                time += Time.deltaTime;
                yield return null;
            }
            comboExplosion.gameObject.SetActive(true);
            comboExplosion.transform.position = endPos;
            comboExplosion.Play();
            Destroy(jellyfish);
            Destroy(popcorn);
        }
    }
    
    static float LinearToS(float t, int slope)
    {
        if (t > 1 || t < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
        return 1 / (1 + Mathf.Pow(t / (1 - t), -slope));
    }

    
    static Vector3 BezierIntrp(Vector3 startPos, Vector3 midPos, Vector3 endPos, float t)
    {
        return (((1 - t) * (1 - t)) * startPos) + (((1 - t) * 2.0f) * t * midPos) + ((t * t) * endPos);
    }
}
