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

    [Serializable]
    public struct NamedSprite {
        public string effectName;
        public Sprite onSprite;
        public Sprite offSprite;
    }
    [SerializeField] private NamedSprite[] effectNamedSprites;
    public Dictionary<string, (Sprite, Sprite)> effectSpritesDict;
    
    public delegate void SliderAction(float f);
    public CanvasGroup objectViewGroup;
    public Slider objectSlider1;
    public Slider objectSlider2;
    public List<Button> effectButtons;
    public Button binaryButton;
    // public Button onOffButton;
    public Sprite onButtonSprite;
    public Sprite offButtonSprite;
    [SerializeField] private CubeScript cubeScript;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        effectSpritesDict = new Dictionary<string, (Sprite, Sprite)>();
        foreach (NamedSprite ns in effectNamedSprites)
        {
            effectSpritesDict[ns.effectName] = (ns.onSprite, ns.offSprite);
        }
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
    

    public void ShowObjectView()
    {
        if (cubeScript.GetIsOpen())
        {
            cubeScript.CloseLibrary();
        }
        StartCoroutine(FadeObjectUI(1, true));
        // objectSlider1.onValueChanged.AddListener(new UnityAction<float>(slider1));
        // objectSlider2.onValueChanged.AddListener(new UnityAction<float>(slider2));
        // switch (instrumentTag)
        // {
        //
        //     case "Chord":
        //         objectSlider1.wholeNumbers = true;
        //         objectSlider1.minValue = 1;
        //         objectSlider1.maxValue = 3;
        //         objectSlider2.wholeNumbers = true;
        //         objectSlider2.minValue = 0;
        //         objectSlider2.maxValue = 1;
        //         break;
        //     case "Sequencer":
        //         objectSlider1.wholeNumbers = false;
        //         objectSlider1.minValue = 0;
        //         objectSlider1.maxValue = 1;
        //         objectSlider2.wholeNumbers = false;
        //         objectSlider2.minValue = 0;
        //         objectSlider2.maxValue = 1;
        //         break;
        //     default:
        //         objectSlider1.wholeNumbers = false;
        //         objectSlider1.minValue = 0;
        //         objectSlider1.maxValue = 1;
        //         objectSlider2.wholeNumbers = false;
        //         objectSlider2.minValue = 0;
        //         objectSlider2.maxValue = 1;
        //         break;
        // }
    }
    
    public void HideObjectView()
    {
        StartCoroutine(FadeObjectUI(0, false));
        objectSlider1.onValueChanged.RemoveAllListeners();
        objectSlider2.onValueChanged.RemoveAllListeners();
        // onOffButton.onClick.RemoveAllListeners();
        binaryButton.onClick.RemoveAllListeners();

        foreach (Button b in effectButtons)
        {
            b.onClick.RemoveAllListeners();
        }
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

    public void HideAllElements()
    {
        objectSlider1.gameObject.SetActive(false);
        objectSlider2.gameObject.SetActive(false);
        binaryButton.gameObject.SetActive(false);
        // onOffButton.gameObject.SetActive(false);
        foreach (Button b in effectButtons)
        {
            b.gameObject.SetActive(false);
        }
    }
}
