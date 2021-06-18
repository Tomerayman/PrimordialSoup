using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private CubeScript menuCubeScript;
    [SerializeField] private GameObject instrumentObject;
    private bool isDragged = false;

    static readonly Vector3 beginningPosition = new Vector3(1, 1, -3);
    // Start is called before the first frame update
    void Start()
    {
        menuCubeScript = GameObject.Find("Menu Cube").GetComponent<CubeScript>();
    }

    public GameObject getNestedInstrument()
    {
        return instrumentObject;
    }

    public void createFromTile()
    {
        GameObject newInstrument = Instantiate(instrumentObject);
        newInstrument.GetComponent<Transform>().position = beginningPosition;
        menuCubeScript.CloseLibrary();
    }

}
