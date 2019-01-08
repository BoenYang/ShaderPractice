using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Wave))]
public class WaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Wave wave = target as Wave;
        if (DrawDefaultInspector())
        {
            wave.GenerateWaveMesh();

            BoxCollider2D collider = wave.gameObject.GetComponent<BoxCollider2D>();
            MeshFilter meshFilter = wave.gameObject.GetComponent<MeshFilter>();
            if (collider)
            {
                collider.size = meshFilter.sharedMesh.bounds.size;
                collider.offset = meshFilter.sharedMesh.bounds.center;
            }
        }
    }

}
