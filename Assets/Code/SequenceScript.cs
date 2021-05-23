using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class SequenceScript : MonoBehaviour
{
    public SoundSynchronizer soundManager;
    [FMODUnity.EventRef]
    public string sound;
    public int bulbsNum;
    public bool isPlaying;
    public float litRatio;
    private float randomRatio;
    public List<MeshRenderer> bulbs;
    private List<int> _offBulbs;
    private List<int> _litBulbs;
    private bool _ratioChanged;
    private UIController _uiController;

    void Start()
    {
        _ratioChanged = false;
        soundManager = GameObject.Find("GameController").GetComponent<SoundSynchronizer>();
        ResetBulbs();
        StartCoroutine(SequenceSoundEmit());
        RandomizeBulbs();
        LibObjectScript libScript = GetComponent<LibObjectScript>();
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
    
    // void CreateBulbs()
    // {
    //     float increment = 2 * Mathf.PI / bulbsNum;
    //     
    //     for (int i = 0; i < bulbsNum; i++)
    //     {
    //         _bulbs.Add(Instantiate(bulbPrefab).GetComponent<MeshRenderer>());
    //         _bulbs[i].transform.parent = _mTransform;
    //         _bulbs[i].transform.localPosition = new Vector3(radius * Mathf.Sin(i * increment), radius * Mathf.Cos(i * increment), 0f);
    //     }
    // }
    
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
            bulbs[_litBulbs[i]].enabled = false;
        }
        
    }
    
    
    // Update is called once per frame
    void Update()
    {
        // _currDispAmount = Mathf.Lerp(_currDispAmount, _minDispAmount, Time.deltaTime);
        // _meshRenderer.material.SetFloat(amountID, _currDispAmount);
            if (_ratioChanged)
            {
                _ratioChanged = false;
                ChangeBulbs();
            }
    }

    public void PlaySound(bool effect)
    {
        // _currDispAmount = 0.3f;
        SoundSynchronizer.SoundData soundData = new SoundSynchronizer.SoundData();
        soundData.sound = sound;
        soundData.customPlayback = false;
        // more sound definitions (effects..)
        soundManager.sounds.Add(soundData);
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

    public void ChangeRandom(float newValue)
    {
        randomRatio = newValue;
    }

    public void ChangeBulbs()
    {
        int bulbsToLight = Mathf.RoundToInt(litRatio * bulbsNum) - _litBulbs.Count;
        if (bulbsToLight > 0) // need to add bulbs
        {
            for (int i = 0; i < bulbsToLight; i++)
            {
                int r = Random.Range(0, _offBulbs.Count);
                _litBulbs.Add(_offBulbs[r]);
                // _bulbs[_offBulbs[r]].material = onMtl;
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
                // _bulbs[_litBulbs[r]].material = offMtl;
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
    }

}