using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {

    public Vector4[] Dirs;
	public float[] Amps;
    public float[] Phases;
    public float[] Frequency;

    private MeshRenderer renderer;

    public void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        renderer.material.SetVectorArray("_Dirs", Dirs);
        renderer.material.SetFloatArray("_Amps", Amps);
        renderer.material.SetFloatArray("_Phases", Phases);
        renderer.material.SetFloatArray("_Frequency", Frequency);
        renderer.material.SetFloat("_Num", Dirs.Length);
    }
}
