using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeScript : MonoBehaviour
{
    [SerializeField] private RectTransform libTransform;
    private Vector3 openLibPos;
    private Vector3 closeLibPos;
    [SerializeField] private CanvasGroup libCanvasGroup;
    private bool isOpen;
    private bool isMidClick;
    [SerializeField] private Transform tileContainer;
    [SerializeField] private Transform bookmarkContainer;
    private List<(Image, TileScript)> tiles;
    private List<Image> bookmarks;
    [SerializeField] private Image baseImage;
    public bool  isDraggingTile = false;
    private Canvas mCanvas;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    
    
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        isMidClick = false;
        openLibPos = libTransform.position;
        closeLibPos = openLibPos + new Vector3(-700, 0, 0);
        libTransform.position = closeLibPos;
        tiles = new List<(Image, TileScript)>();
        foreach (Transform tile in tileContainer)
        {
            tiles.Add((tile.GetComponent<Image>(), tile.GetComponent<TileScript>()));
        }
        bookmarks = new List<Image>();
        foreach (Transform bookmark in bookmarkContainer)
        {
            bookmarks.Add(bookmark.GetComponent<Image>());
        }
        mCanvas = transform.GetChild(0).GetComponent<Canvas>();
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
                bool libraryClick = false;
                foreach (var result in results)
                {
                    if (ReferenceEquals(result.gameObject, baseImage.gameObject)) // click raycast hit menu base
                    {
                        libraryClick = true;
                    }
                    else
                    {
                        foreach (Image bookmark in bookmarks)
                            if (ReferenceEquals(result.gameObject, bookmark.gameObject))
                            {
                                libraryClick = true;
                            }
                    }
                }

                if (libraryClick)
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
        StartCoroutine(EnlargeLibrary(openLibPos, true));
    }
    
    public void CloseLibrary()
    {
        isOpen = false;
        StartCoroutine(EnlargeLibrary(closeLibPos, false));
        
    }

    IEnumerator EnlargeLibrary(Vector3 pos, bool interactable)
    {
        float time = 0;
        float duration = 0.5f;
        while (time < duration)
        {
            libTransform.position = Vector3.Lerp(libTransform.position, pos, LinearToS(time / duration, 3));
            // libCanvasGroup.alpha = Mathf.Lerp(libCanvasGroup.alpha, newAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        libTransform.position = pos;
        // libCanvasGroup.alpha = newAlpha;
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
    
    static float LinearToS(float t, int slope)
    {
        if (t > 1 || t < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
        return 1 / (1 + Mathf.Pow(t / (1 - t), -slope));
    }
}
