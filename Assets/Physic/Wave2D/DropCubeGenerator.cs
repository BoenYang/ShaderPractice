using System.Collections;
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
            Vector3 rot = new Vector3(0, 0, Random.Range(0, 360));
            Vector3 scale = new Vector3(Random.Range(1,3), Random.Range(1, 3), Random.Range(1, 3));
            GameObject go = Instantiate(cube,pos, Quaternion.Euler(rot));
            go.transform.localScale = scale;
        }
    }
}
