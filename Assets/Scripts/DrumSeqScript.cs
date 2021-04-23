using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumSeqScript : MonoBehaviour
{
    public GameObject bulbPrefab;
    public int bulbsNum;
    public float radius;
    public bool isPlaying;
    public float litRatio;
    private List<SpriteRenderer> bulbs;
    private List<int> offBulbs;
    private List<int> litBulbs;
    private Transform mTransform;
    private SoundControllerScript soundController;


    [FMODUnity.EventRef]
    public string sound;

    public FMOD.Studio.EventInstance soundEvent;

    public Sprite offBulbIm;
    public Sprite litBulbIm;

    // Start is called before the first frame update
    void Start()
    {
        mTransform = GetComponent<Transform>();
        soundEvent = FMODUnity.RuntimeManager.CreateInstance(sound);
        soundController = GameObject.Find("SoundController").GetComponent<SoundControllerScript>();
        bulbs = new List<SpriteRenderer>();
        resetBulbs();
        createBulbs();
        StartCoroutine(SequenceSoundEmit());
        RandomizeBulbs();
    }

    private void resetBulbs()
    {
        litBulbs = new List<int>();
        offBulbs = new List<int>();
        for (int i = 0; i < bulbsNum; i++)
        {
            offBulbs.Add(i);
        }
    }

    private void RandomizeBulbs()
    {
        int bulbsToLight = Mathf.RoundToInt(litRatio * bulbsNum);

        for (int i = 0; i < bulbsToLight; i++)
        {
            int r = Random.Range(0, offBulbs.Count);
            litBulbs.Add(offBulbs[r]);
            offBulbs.RemoveAt(r);
        }
        for (int i = 0; i < litBulbs.Count; i++)
        {
            bulbs[litBulbs[i]].sprite = litBulbIm;
        }
        for (int i = 0; i < offBulbs.Count; i++)
        {
            bulbs[offBulbs[i]].sprite = offBulbIm;
        }
    }

    void createBulbs()
    {
        float increment = 2 * Mathf.PI / bulbsNum;
        
        for (int i = 0; i < bulbsNum; i++)
        {
            bulbs.Add(Instantiate(bulbPrefab).GetComponent<SpriteRenderer>());
            bulbs[i].transform.parent = mTransform;
            bulbs[i].transform.localPosition = new Vector3(radius * Mathf.Cos(i * increment), radius * Mathf.Sin(i * increment), 0f);
        }
    }

    IEnumerator SequenceSoundEmit()
    {
        int i = 0;
        while(true)
        {
            if (isPlaying)
            {
                if (litBulbs.Contains(i))
                {
                    soundController.sounds.Add((soundEvent, false));
                    Debug.Log("now " + i);
                }
                i = (i < bulbsNum) ? i + 1 : 0;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
