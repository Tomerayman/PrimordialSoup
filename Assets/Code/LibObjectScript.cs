using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LibObjectScript : MonoBehaviour
{
    public bool isDraggable = true;
    public bool isDragged = false;
    public string instrumentTag;
    public UIController.SliderAction slider1Action;
    public UIController.SliderAction slider2Action;
    private Plane mouseProjPlane;
    private Camera mainCamera;
    float mouseRayDistance;
    Vector3 mousePos;
    private double _lastTapTime;
    private CameraScript _cameraScript;
    private Transform activeInstruments;
    private Vector3 pushBackVector = new Vector3(0, 0, 20);
    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mouseProjPlane = new Plane(Vector3.back, transform.position);
        _lastTapTime = Time.timeAsDouble;
        _cameraScript = mainCamera.gameObject.GetComponent<CameraScript>();
        activeInstruments = GameObject.Find("ActiveInstruments").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        mouseProjPlane.Raycast(ray, out mouseRayDistance);
        mousePos = ray.GetPoint(mouseRayDistance);
        if (isDragged)
        {
            //var position = transform.position;
            ////position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(Input.mousePosition);
            //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            transform.position = mousePos;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            double currTapTime = Time.timeAsDouble;
            if (currTapTime - _lastTapTime < 0.2) // double tap
            {
                if (!_cameraScript.zoomedIn)
                {
                    _cameraScript.ZoomAtInstrument(transform.position, slider1Action, slider2Action, instrumentTag);
                    soloDisplay(false);
                    isDraggable = false;
                }
                else
                {
                    _cameraScript.ZoomOut();
                    soloDisplay(true);
                    isDraggable = true;
                }
            }
            else if (!_cameraScript.zoomedIn && isDraggable)
            {
                StartCoroutine(dragDelay());
            }
            _lastTapTime = currTapTime;
        }
    }

    IEnumerator dragDelay()
    {
        yield return new WaitForSeconds(0.25f);
        if (isDraggable)
        {
            isDragged = true;
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        if (instrumentTag == "Filter")
        {
            
        }
    }
    
    private void soloDisplay(bool reverse)
    {
        int reverseFactor = (reverse) ? -1 : 1;
        foreach (Transform instrumentTransform in activeInstruments)
        {
            if (! ReferenceEquals(instrumentTransform, transform))
            {
                instrumentTransform.position += reverseFactor * pushBackVector;
            }
        }
    }
}


