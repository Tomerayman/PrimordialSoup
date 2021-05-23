using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public delegate void SliderAction(float f);
    public CanvasGroup objectViewGroup;
    [SerializeField]
    private Slider objectSlider1;
    [SerializeField]
    private Slider objectSlider2;
    public float val1;
    public float val2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /**
     * chord:
     *  - slider1: 3 modes
     *  - slider2: 2 modes
     *
     * sample:
     *  - slider1: float
     *  - slider2: float
     *
     * sequencer:
     *  - slider1: float
     *  - slider2: float
     */
    

    public void ShowObjectView(SliderAction slider1, SliderAction slider2, string instrumentTag)
    {
        StartCoroutine(FadeObjectUI(1, true));
        objectSlider1.onValueChanged.AddListener(new UnityAction<float>(slider1));
        objectSlider2.onValueChanged.AddListener(new UnityAction<float>(slider2));
        switch (instrumentTag)
        {

            case "Chord":
                objectSlider1.wholeNumbers = true;
                objectSlider1.minValue = 1;
                objectSlider1.maxValue = 3;
                objectSlider2.wholeNumbers = true;
                objectSlider2.minValue = 0;
                objectSlider2.maxValue = 1;
                break;
            case "Sequencer":
                objectSlider1.wholeNumbers = false;
                objectSlider1.minValue = 0;
                objectSlider1.maxValue = 1;
                objectSlider2.wholeNumbers = false;
                objectSlider2.minValue = 0;
                objectSlider2.maxValue = 1;
                break;
            default:
                objectSlider1.wholeNumbers = false;
                objectSlider1.minValue = 0;
                objectSlider1.maxValue = 1;
                objectSlider2.wholeNumbers = false;
                objectSlider2.minValue = 0;
                objectSlider2.maxValue = 1;
                break;
        }
    }
    
    public void HideObjectView()
    {
        StartCoroutine(FadeObjectUI(0, false));
        objectSlider1.onValueChanged.RemoveAllListeners();
        objectSlider2.onValueChanged.RemoveAllListeners();
    }
    
    IEnumerator FadeObjectUI(float newAlpha, bool interactable)
    {
        float time = 0;
        float duration = 0.5f;
        while (time < duration)
        {
            objectViewGroup.alpha = Mathf.Lerp(objectViewGroup.alpha, newAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        objectViewGroup.alpha = newAlpha;
        objectViewGroup.interactable = interactable;
    }
}
