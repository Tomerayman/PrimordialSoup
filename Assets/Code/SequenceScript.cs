using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SequenceScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    [FMODUnity.EventRef]
    public string sound;
    public GameObject bulbPrefab;
    public int bulbsNum;
    public float radius;
    public bool isPlaying;
    public float litRatio;
    private List<MeshRenderer> _bulbs;
    private List<int> _offBulbs;
    private List<int> _litBulbs;
    private Transform _mTransform;
    private bool _ratioChanged;
    public Material onMtl;
    public Material offMtl;
    public Transform dialTransform;



    void Start()
    {
        _mTransform = GetComponent<Transform>();
        _ratioChanged = false;
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        // StartCoroutine(test());
        _bulbs = new List<MeshRenderer>();
        ResetBulbs();
        CreateBulbs();
        StartCoroutine(SequenceSoundEmit());
        RandomizeBulbs();
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
    
    void CreateBulbs()
    {
        float increment = 2 * Mathf.PI / bulbsNum;
        
        for (int i = 0; i < bulbsNum; i++)
        {
            _bulbs.Add(Instantiate(bulbPrefab).GetComponent<MeshRenderer>());
            _bulbs[i].transform.parent = _mTransform;
            _bulbs[i].transform.localPosition = new Vector3(radius * Mathf.Sin(i * increment), radius * Mathf.Cos(i * increment), 0f);
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
            _bulbs[_litBulbs[i]].material = onMtl;
        }
        for (int i = 0; i < _offBulbs.Count; i++)
        {
            _bulbs[_offBulbs[i]].material = offMtl;
        }
    }
    
    
    // Update is called once per frame
    void Update()
        {
            // dialTransform.Rotate(new Vector3(0, 0, 90 * Time.deltaTime));
            if (_ratioChanged)
            {
                _ratioChanged = false;
                ChangeBulbs();
            }
        }

    public void PlaySound(bool effect)
    {
        soundManager.sounds.Add((sound, effect));
    }
    
    IEnumerator SequenceSoundEmit()
    {
        int i = 0;
        while(true)
        {
            if (isPlaying)
            {
                Debug.Log(i);
                dialTransform.Rotate(new Vector3(0f, 0f, -(360f / bulbsNum)));
                if (_litBulbs.Contains(i))
                {
                    PlaySound(false);
                }
                i = (i < bulbsNum - 1) ? i + 1 : 0;
                yield return new WaitForSeconds(4f / bulbsNum);
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

    public void ChangeBulbs()
    {
        Debug.Log("got here " + litRatio);
        int bulbsToLight = Mathf.RoundToInt(litRatio * bulbsNum) - _litBulbs.Count;
        if (bulbsToLight > 0) // need to add bulbs
        {
            for (int i = 0; i < bulbsToLight; i++)
            {
                int r = Random.Range(0, _offBulbs.Count);
                _litBulbs.Add(_offBulbs[r]);
                _bulbs[_offBulbs[r]].material = onMtl;
                _offBulbs.RemoveAt(r);
            }
        }
        else if (bulbsToLight < 0) // need to turn off bulbs
        {
            for (int i = 0; i < -bulbsToLight; i++)
            {
                int r = Random.Range(0, _litBulbs.Count);
                _offBulbs.Add(_litBulbs[r]);
                _bulbs[_litBulbs[r]].material = offMtl;
                _litBulbs.RemoveAt(r);
            }
        }
        
    }

    // IEnumerator test()
    // {
    //     while (true)
    //     {
    //         playSound(false);
    //         yield return new WaitForSeconds(1f);
    //     }
    // }

}