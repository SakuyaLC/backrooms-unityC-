using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Connections")]
    public Transform CLeft;
    public bool UsedLeft = false;

    public Transform CUpper;
    public bool UsedUpper = false;

    public Transform CRight;
    public bool UsedRight = false;

    public Transform CBottom;
    public bool UsedBottom = false;

    [Header("Misc")]
    public GameObject forRender;

    private void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }

}
