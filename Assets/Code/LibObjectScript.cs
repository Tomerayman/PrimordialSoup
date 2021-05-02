using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibObjectScript : MonoBehaviour
{
    public bool isDraggable = true;
    public bool isDragged = false;
    Plane mouseProjPlane;
    private Camera mainCamera;
    float mouseRayDistance;
    Vector3 mousePos;



    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mouseProjPlane = new Plane(Vector3.back, transform.position);
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
        if (isDraggable && Input.GetMouseButtonDown(0))
        {
            isDragged = true;
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
    }
}


