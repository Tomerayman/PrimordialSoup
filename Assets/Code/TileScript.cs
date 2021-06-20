using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [SerializeField] private GameObject instrumentObject;

    // Start is called before the first frame update
    void Start()
    {
    }

    public GameObject getNestedInstrument()
    {
        return instrumentObject;
    }
}
