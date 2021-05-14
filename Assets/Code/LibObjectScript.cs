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
    

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mouseProjPlane = new Plane(Vector3.back, transform.position);
        _lastTapTime = Time.timeAsDouble;
        _cameraScript = mainCamera.gameObject.GetComponent<CameraScript>();
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
                    isDraggable = false;
                }
                else
                {
                    _cameraScript.ZoomOut();
                    isDraggable = true;
                }
            }
            else if (isDraggable)
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
    }
}


