using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoopLinkScript : MonoBehaviour
{
    // Start is called before the first frame update
    
    public bool isEmpty = true;
    public bool isNestingChord = false;
    public string sound;
    public List<string> nestedChordSounds;
    private LibObjectScript parentLibScript;
    private LoopScript parentLoopScript;
    [SerializeField] private MeshRenderer emptyRenderer;
    private GameObject nestedObject;
    private MeshRenderer filledRenderer;

    public static float nestedSampleScaleRatio = 0.5f;
    public static float nestedChordScaleRatio = 0.3f;
    
    void Start()
    {
        filledRenderer = GetComponent<MeshRenderer>();
        TurnLinkEmpty();
        parentLibScript = transform.parent.GetComponent<LibObjectScript>();
        parentLoopScript = transform.parent.GetComponent<LoopScript>();
    }

    private void TurnLinkEmpty()
    {
        filledRenderer.enabled = false;
        emptyRenderer.enabled = true;
    }
    
    private void TurnLinkFull()
    {
        filledRenderer.enabled = true;
        emptyRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GiveSound()
    {
        if (!isNestingChord)
        {
            return sound;
        }
        // currently randomizing over sounds
        int i = Random.Range(0, nestedChordSounds.Count);
        return nestedChordSounds[i];
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Sample") || other.CompareTag("Chord")) &&
            !parentLibScript.getIsDragged() &&
            other.GetComponent<LibObjectScript>().getIsDragged())
        {
            // StartCoroutine(WaitForObjectRelease(other.gameObject));
            parentLoopScript.AddLinkCandidate(transform, other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Sample") || other.CompareTag("Chord")) &&
            !parentLibScript.getIsDragged() &&
            other.GetComponent<LibObjectScript>().getIsDragged())
        {
            parentLoopScript.RemoveLinkCandidate(transform);
        }
    }

    // IEnumerator WaitForObjectRelease(GameObject sampleObject)
    // {
    //     yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
    //     if (sampleObject != null &&
    //         Vector3.Distance(sampleObject.transform.position, transform.position) <
    //         GetComponent<CapsuleCollider>().height * transform.localScale.x)
    //     {
    //         nestObject(sampleObject);
    //     }
    // }
    
    public float ValidateObjectDistance(Transform obj)
    {
        float distance = Vector3.Distance(obj.position, transform.position);
        if (distance > GetComponent<CapsuleCollider>().height * transform.localScale.x)
        {
            return -1;
        }
        return distance;
    }
    
    public void FillWithObject(GameObject sampleObject)
    {
        Destroy(nestedObject);
        nestedObject = sampleObject;
        isEmpty = false;
        TurnLinkFull();
        nestedObject.transform.parent = transform;
        nestedObject.transform.localPosition = Vector3.zero;
        nestedObject.GetComponent<LibObjectScript>().isDraggable = false;
        nestedObject.GetComponent<Collider>().enabled = false;
        if (sampleObject.CompareTag("Sample"))
        {
            isNestingChord = false;
            SampleScript sampleScript = nestedObject.GetComponent<SampleScript>();
            sound = sampleScript.sound;
            nestedObject.transform.localScale = nestedObject.transform.localScale * nestedSampleScaleRatio; 
        }
        else if (sampleObject.CompareTag("Chord"))
        {
            isNestingChord = true;
            nestedChordSounds = nestedObject.GetComponent<ChordScript>().sounds;
            nestedObject.transform.localScale = nestedObject.transform.localScale * nestedChordScaleRatio; 
        }
        
    }
}
