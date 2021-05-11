using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class LibraryScript : MonoBehaviour
{
    bool libraryShown = true;
    public Vector2 showPos;
    public Vector2 hidePos;
    public RectTransform mRect, showHideButtonRect;

    
    // Start is called before the first frame update
    void Start()
    {
        mRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showHideButtonClick()
    {
        if (libraryShown)
        {
            mRect.DOAnchorPos(hidePos, 0.25f);
            showHideButtonRect.DORotate(new Vector3(0, 0, 0), 0.5f);
        }
        else
        {
            mRect.DOAnchorPos(showPos, 0.25f);
            showHideButtonRect.DORotate(new Vector3(0, 0, 180), 0.5f);
        }
        libraryShown = !libraryShown;
    }



}
