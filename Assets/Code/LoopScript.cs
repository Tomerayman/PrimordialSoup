using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    private UIController _uiController;
    public bool isPlaying;
    public bool isFilling;
    public float cycleDuration;
    private Transform pendingFillingObject;
    [SerializeField] List<LoopLinkScript> links;
    private List<Material> pulses;
    private static readonly int pulseID = Shader.PropertyToID("_Alpha");
    [SerializeField] private int linksNum;
    [SerializeField] private float rotationSpeed;
    private List<Transform> linkCandidatesForFilling;
    private LibObjectScript libScript;


    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        _uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
        linkCandidatesForFilling = new List<Transform>();
        libScript = GetComponent<LibObjectScript>();
        InitPulses();
        StartCoroutine(SequenceSoundEmit());
    }
    
    private void InitPulses()
    {
        pulses = new List<Material>();
        for (int i = 0; i < 8; i++)
        {
            Material mtl = transform.Find("Link" + i.ToString()).Find("PULSE").GetComponent<MeshRenderer>().material;
            mtl.SetFloat(pulseID, 0);
            pulses.Add(mtl);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        if (isFilling && Input.GetMouseButtonUp(0))
        {
            isFilling = false;
            FillClosestLink();
            pendingFillingObject = null;
        }
    }
    
    public void PlaySound(SoundSynchronizer.SoundData soundData)
    {
        // _currDispAmount = 0.3f;
        // SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        // soundData.sound = sound;
        soundData.volume = libScript.GetVolumeFromScale();
        // more sound definitions (effects..)
        
        // soundManager.sounds.Add(soundData);
        soundManager.SimplePlay(soundData);
    }
    
    IEnumerator SequenceSoundEmit()
    {
        int i = 0;
        while(true)
        {
            if (isPlaying)
            {
                if (!links[i].isEmpty)
                {
                    StartCoroutine(PulseCycle(pulses[i]));
                    PlaySound(links[i].GiveSound());
                }

                if (i < linksNum - 1)
                {
                    i++;
                }
                else
                {
                    i = 0;
                }
                yield return new WaitForSeconds(cycleDuration / linksNum);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void AddLinkCandidate(Transform link, Transform fillingObject)
    {
        isFilling = true;
        if (!linkCandidatesForFilling.Contains(link))
        {
            linkCandidatesForFilling.Add(link);    
        }
        pendingFillingObject = fillingObject;
    }

    public void RemoveLinkCandidate(Transform link)
    {
        linkCandidatesForFilling.Remove(link);
        if (linkCandidatesForFilling.Count == 0)
        {
            isFilling = false;
            pendingFillingObject = null;
        }
    }

    private void FillClosestLink()
    {
        if (linkCandidatesForFilling.Count == 0)
        {
            isFilling = false;
            pendingFillingObject = null;
        }
        Transform closestLink = null;
        float closestDistance = Mathf.Infinity;
        foreach (var link in linkCandidatesForFilling)
        {
            float currLinkDistance = link.GetComponent<LoopLinkScript>().ValidateObjectDistance(pendingFillingObject);
            if (currLinkDistance > 0 && currLinkDistance < closestDistance)
            {
                closestLink = link;
                closestDistance = currLinkDistance;
            }
        }
        if (closestLink != null && pendingFillingObject != null)
        {
            closestLink.GetComponent<LoopLinkScript>().FillWithObject(pendingFillingObject.gameObject);
        }
    }

    public void ChangeSpeed(float value)
    {
        cycleDuration = 1 + (1 - value) * 7;
        rotationSpeed = 9 - cycleDuration;
    }
    
    public void SetSequencerUI()
    {
        _uiController.HideAllElements();
        _uiController.objectSlider1.gameObject.SetActive(true);
        _uiController.objectSlider1.value = (8 - cycleDuration) / 7;
        _uiController.objectSlider1.onValueChanged.AddListener(ChangeSpeed);
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
