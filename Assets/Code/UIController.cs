using System;
using System.Collections;
using System.Collections.Generic;
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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void ShowObjectView(SliderAction slider1, SliderAction slider2)
    {
        StartCoroutine(FadeObjectUI(1, true));
        objectSlider1.onValueChanged.AddListener(new UnityAction<float>(slider1));
        objectSlider2.onValueChanged.AddListener(new UnityAction<float>(slider2));
        
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
