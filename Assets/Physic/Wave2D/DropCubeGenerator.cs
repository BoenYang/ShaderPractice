﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCubeGenerator : MonoBehaviour
{

    public GameObject cube;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPos = Input.mousePosition;
            Vector3 pos = Camera.main.ScreenToWorldPoint(touchPos);
            pos.z = 0;
            Instantiate(cube,pos, Quaternion.identity);
        }
    }
}