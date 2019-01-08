using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCube : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Wave wave = collision.GetComponent<Wave>();
        if (wave != null)
        {
            
        }
    }

}
