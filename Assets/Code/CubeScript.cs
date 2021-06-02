using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{
    [SerializeField] private Transform libTransform;
    [SerializeField] private CanvasGroup libCanvasGroup;
    private bool isOpen;
    private bool isMidClick; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isMidClick = false;
        libTransform.localScale = new Vector3(0, 0, 0);
        libCanvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    private void OnMouseOver()
    {
        IEnumerator clickDelay()
        {
            yield return new WaitForSeconds(0.2f);
            isMidClick = false;
        }
        
        if (Input.GetMouseButtonDown(0) && !isMidClick)
        {
            isMidClick = true;
            StartCoroutine(clickDelay());
            if (isOpen)
            {
                CloseLibrary();
            }
            else
            {
                OpenLibrary();
            }
        }
    }

    private void OpenLibrary()
    {
        isOpen = true;
        StartCoroutine(EnlargeLibrary(1, 1, true));
    }
    
    public void CloseLibrary()
    {
        isOpen = false;
        StartCoroutine(EnlargeLibrary(0, 0, false));
    }

    IEnumerator EnlargeLibrary(float newScale, float newAlpha, bool interactable)
    {
        float time = 0;
        float duration = 0.5f;
        Vector3 newScaleVect = new Vector3(newScale, newScale, newScale);
        while (time < duration)
        {
            libTransform.localScale = Vector3.Lerp(libTransform.localScale, newScaleVect, time / duration);
            libCanvasGroup.alpha = Mathf.Lerp(libCanvasGroup.alpha, newAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        libTransform.localScale = newScaleVect;
        libCanvasGroup.alpha = newAlpha;
        libCanvasGroup.interactable = interactable;
    }
}
