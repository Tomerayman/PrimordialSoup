using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class LibObjectScript : MonoBehaviour
{
    public bool isDraggable = true;
    public string instrumentTag;
    public float maxScaleRatio;
    public float minScaleRatio;
    public float currScaleRatio;
    private bool isDragged = false;
    private bool isShrinking = false;
    private bool isPinched = false;
    private Plane mouseProjPlane;
    private Camera mainCamera;
    private float mouseRayDistance;
    private Vector3 mousePos;
    private double _lastTapTime;
    private CameraScript _cameraScript;
    private Transform activeInstruments;
    private float initialDistance;
    private Vector3 initialScale;
    private Transform mTransform;
    private Transform menuCubeTransform;
    private static float speed = 0.3f;
    private static float posFloatRange = 0.1f;
    private static float rotFloatRange = 2;
    private static float shrinkageAreaThreshold = 1;
    private static float disposalAreaThreshold = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mTransform = GetComponent<Transform>();
        mouseProjPlane = new Plane(Vector3.back, transform.position);
        _lastTapTime = Time.timeAsDouble;
        _cameraScript = mainCamera.gameObject.GetComponent<CameraScript>();
        activeInstruments = GameObject.Find("ActiveInstruments").GetComponent<Transform>();
        mTransform.parent = activeInstruments;
        menuCubeTransform = GameObject.Find("Menu Cube").GetComponent<Transform>();
        currScaleRatio = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        mouseProjPlane.Raycast(ray, out mouseRayDistance);
        mousePos = ray.GetPoint(mouseRayDistance);
        // floating movements:
        if(isDraggable) {FloatBehaviour();}
        if (isDragged)
        {
            if (Input.GetMouseButton(0))
            {
                transform.position = mousePos;
            }
            else
            {
                isDragged = false;
            }
            // handling library disposing:
            if (!isShrinking && Vector3.Distance(transform.position, menuCubeTransform.position) < shrinkageAreaThreshold)
            {
                isShrinking = true;
                StartCoroutine(ShrinkAreaBehaviour());
            }
        }
        if (Input.touchCount == 2) // detecting pinch action
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            // if the pinch is now ending:
            if (isPinched && (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled))
            {
                isPinched = false;
                return;
            }
            // if pinch just began: 
            if ((touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began) && checkPinch(touchZero, touchOne))
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            else if (isPinched) // we are mid-pinch:
            {
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
                if (Mathf.Approximately(initialDistance, 0)) // if pinch is very small / accidental
                {
                    return;
                }

                var factor = currentDistance / initialDistance;
                if (factor > maxScaleRatio)
                    factor = maxScaleRatio;
                else if (factor < minScaleRatio)
                    factor = minScaleRatio;
                currScaleRatio = factor;
                transform.localScale = initialScale * currScaleRatio;
            }
        }
    }

    private bool checkPinch(Touch pointZero, Touch pointOne)
    {
        Collider selfCollider = GetComponent<Collider>(); 
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(pointZero.position), out hit, 50) && ReferenceEquals(hit.collider, selfCollider))
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(pointOne.position), out hit, 50) &&
                ReferenceEquals(hit.collider, selfCollider))
            {
                isPinched = true;
                return true;
            }
        }
        return false;
    }

    public bool getIsDragged()
    {
        return isDragged;
    }

    private void FloatBehaviour()
    {
        var position = mTransform.position;
        float lerpPoint = Time.time * speed + 10*position.x + 10*position.y; 
        // position += Vector3.forward * (Time.deltaTime * Mathf.Sin(lerpPoint) * posFloatRange);
        // mTransform.position = position;
        transform.Rotate(Vector3.up * (Time.deltaTime * rotFloatRange * Mathf.Sin(lerpPoint)));
        transform.Rotate(Vector3.left * (Time.deltaTime * rotFloatRange * Mathf.Sin(lerpPoint)));
    }
    
    IEnumerator ShrinkAreaBehaviour()
    {
        var distanceToCube = Vector3.Distance(mTransform.position, menuCubeTransform.position);
        Vector3 originalScale = mTransform.localScale;
        while (isDragged && distanceToCube < shrinkageAreaThreshold)
        {
            if (distanceToCube < disposalAreaThreshold)
            {
                disposeObject();
                break;
            }
            mTransform.localScale = originalScale * (distanceToCube / shrinkageAreaThreshold);
            yield return new WaitForEndOfFrame();
            distanceToCube = Vector3.Distance(mTransform.position, menuCubeTransform.position);
            
        }
        mTransform.localScale = originalScale;
        isShrinking = false;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            double currTapTime = Time.timeAsDouble;
            if (currTapTime - _lastTapTime < 0.25) // double tap
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
            else if (!_cameraScript.zoomedIn)
            {
                StartCoroutine(dragDelay());
            }
            _lastTapTime = currTapTime;
        }
    }

    public void disposeObject()
    {
        // later - pooling.
        GameObject.Destroy(this.gameObject);
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
            case "Loop":
                GetComponent<LoopScript>().SetSequencerUI();
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
    
    private void soloDisplay(bool isZoomingOut)
    {
        // int reverseFactor = (reverse) ? -1 : 1;
        foreach (Transform instrumentTransform in activeInstruments)
        {
            if (! ReferenceEquals(instrumentTransform, transform))
            {
                // instrumentTransform.position += reverseFactor * pushBackVector;
                Renderer[] rs = instrumentTransform.GetComponentsInChildren<Renderer>();
                foreach(Renderer r in rs)
                    r.enabled = isZoomingOut;
            }
        }

        if (isZoomingOut)
        {
            foreach (Transform instrumentTransform in activeInstruments)
            {
                if (instrumentTransform.CompareTag("Sequencer"))
                {
                    instrumentTransform.GetComponent<SequenceScript>().DerenderOffBulbs();
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }

    public float GetVolumeFromScale()
    {
        float volume = 0.5f + 0.25f * (Mathf.Min(1, Mathf.Max((currScaleRatio - minScaleRatio) / (1 - minScaleRatio), 0)) +
                                       Mathf.Max((currScaleRatio - 1) / (maxScaleRatio - 1), 0)); 
        Debug.Log(volume);
        return volume;
    }
}


