using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraScript : MonoBehaviour
{
    public float defaultInstrumentSize;
    private Camera _camera;
    public float wideViewSize;
    [SerializeField] private float duration;
    public bool zoomedIn = false;
    private UIController uiController;
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
        uiController = GameObject.Find("Game_UI").GetComponent<UIController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ZoomAtInstrument(Vector3 pos)
    {
        ZoomAtInstrument(pos, defaultInstrumentSize);
    }

    void ZoomAtInstrument(Vector3 pos, float size)
    {
        zoomedIn = true;
        Vector3 newPos = new Vector3(pos.x, pos.y, -10);
        StartCoroutine(InterpolateCamera(newPos, size, duration));
        uiController.ShowObjectView();
    }

    public void ZoomOut()
    {
        zoomedIn = false;
        Vector3 origin = new Vector3(0, 0, -10);
        StartCoroutine(InterpolateCamera(origin, wideViewSize, duration));
        uiController.HideObjectView();
    }

    IEnumerator InterpolateCamera(Vector3 newPos,float newSize, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, time / duration);
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, newSize,
                time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = newPos;
        _camera.orthographicSize = newSize;
    }
}
