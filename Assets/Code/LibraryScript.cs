using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class LibraryScript : MonoBehaviour
{
    bool libraryShown = true;
    static Vector2 showPos = new Vector2(5, 0);
    static Vector2 hidePos = new Vector2(3.72f, 0);
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
            showHideButtonRect.DORotate(new Vector3(0, 0, 0), 0.25f);
        }
        else
        {
            mRect.DOAnchorPos(showPos, 0.25f);
            showHideButtonRect.DORotate(new Vector3(0, 0, 180), 0.25f);
        }
        libraryShown = !libraryShown;
    }



}
