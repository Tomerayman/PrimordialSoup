using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

public class LibObjectScript : MonoBehaviour
{
    public bool isDraggable = true;
    public bool isDragged = false;
    public string instrumentTag;
    private Plane mouseProjPlane;
    private Camera mainCamera;
    float mouseRayDistance;
    Vector3 mousePos;
    private double _lastTapTime;
    private CameraScript _cameraScript;
    private Transform activeInstruments;
    private Vector3 pushBackVector = new Vector3(0, 0, 20);
    private float initialDistance;
    private Vector3 initialScale;
    private Transform mTransform;
    private static float speed = 0.3f;
    private static float posFloatRange = 0.1f;
    private static float rotFloatRange = 2;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mTransform = GetComponent<Transform>();
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
        // floating movements:
        var position = mTransform.position;
        float lerpPoint = Time.time * speed + 10*position.x + 10*position.y; 
        position += Vector3.forward * (Time.deltaTime * Mathf.Sin(lerpPoint) * posFloatRange);
        mTransform.position = position;
        transform.Rotate(Vector3.up * (Time.deltaTime * rotFloatRange * Mathf.Sin(lerpPoint)));
        transform.Rotate(Vector3.left * (Time.deltaTime * rotFloatRange * Mathf.Sin(lerpPoint)));
        if (isDragged)
        {
            //var position = transform.position;
            ////position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log(Input.mousePosition);
            //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            transform.position = mousePos;
        }
        if (Input.touchCount == 2) // detecting pinch action
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            // if the pinch is now ending:
            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                return;
            }
            // if pinch just began: 
            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            else // we are mid-pinch:
            {
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
                if (Mathf.Approximately(initialDistance, 0)) // if pinch is very small / accidental
                {
                    return;
                }

                var factor = currentDistance / initialDistance;
                transform.localScale = initialScale * factor;
            }
            
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
                    ZoomInAction();
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

    void ZoomInAction()
    {
        _cameraScript.ZoomAtInstrument(transform.position);
        soloDisplay(false);
        isDraggable = false;
        switch (instrumentTag)
        {
            case "Sample":
                GetComponent<SampleScript>().SetSampleUI();
                break;
            case "Chord":
                GetComponent<ChordScript>().SetChordUI();
                break;
            case "Sequencer":
                GetComponent<SequenceScript>().SetSequencerUI();
                break;
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


