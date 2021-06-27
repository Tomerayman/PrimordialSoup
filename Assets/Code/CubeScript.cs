using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeScript : MonoBehaviour
{
    [SerializeField] private Transform libTransform;
    [SerializeField] private CanvasGroup libCanvasGroup;
    private bool isOpen;
    private bool isMidClick;
    [SerializeField] private Transform tileContainer;
    private List<(Image, TileScript)> tiles;
    [SerializeField] private Image baseImage;
    public bool  isDraggingTile = false;
    [SerializeField] private Canvas mCanvas;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    
    
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isMidClick = false;
        libTransform.localScale = new Vector3(0, 0, 0);
        libCanvasGroup.alpha = 0;
        tiles = new List<(Image, TileScript)>();
        foreach (Transform tile in tileContainer)
        {
            tiles.Add((tile.GetComponent<Image>(), tile.GetComponent<TileScript>()));
        }
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = mCanvas.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = mCanvas.GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            if (!isMidClick) // check if clicked on menu base
            {
                List<RaycastResult> results = rayCastResults();
                foreach (var result in results)
                {
                    if (ReferenceEquals(result.gameObject, baseImage.gameObject)) // click raycast hit menu base
                    {
                        isMidClick = true;
                        StartCoroutine(ClickDelay());
                        if (isOpen)
                        {
                            CloseLibrary();
                        }
                        else
                        {
                            OpenLibrary();
                        }

                        return;
                    }
                }
                if (isOpen)
                {
                    foreach ((Image, TileScript) tile in tiles)
                    {
                        foreach (var result in results)
                        {
                            if (ReferenceEquals(tile.Item1.gameObject, result.gameObject)) // clicked on tile
                            {
                                isDraggingTile = true;
                                StartCoroutine(dragTile(tile.Item1.gameObject, tile.Item2.getNestedInstrument()));
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

        private List<RaycastResult> rayCastResults()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        return results;
    }
    
    public bool GetIsOpen()
    {
        return isOpen;
    }

    IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(0.2f);
        isMidClick = false;
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

    IEnumerator dragTile(GameObject tile, GameObject nestedObject)
    {
        Image tileIcon = tile.transform.GetComponent<Image>();
        Vector3 startPos = tileIcon.rectTransform.position;
        while (Input.GetMouseButton(0)) // during drag
        {
            tileIcon.rectTransform.position = Input.mousePosition;
            yield return new WaitForEndOfFrame();
        }
        if (Vector3.Distance(startPos, Input.mousePosition) < 110) // dropped back. 115 is avg screen radius of tile.
        {
            // returned
        }
        else
        {
            GameObject newInstrument = Instantiate(nestedObject);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            new Plane(Vector3.back, transform.position).Raycast(ray, out distance);
            newInstrument.transform.position = ray.GetPoint(distance);
            CloseLibrary();    
        }

        isDraggingTile = false;
        tileIcon.rectTransform.position = startPos;
    }
}
