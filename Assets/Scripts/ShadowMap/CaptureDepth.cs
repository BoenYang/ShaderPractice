using UnityEngine;
using System.Collections;

public class CaptureDepth : MonoBehaviour
{

    private Shader depthShader;

    private Camera lightCamera;

	// Use this for initialization
	void Start ()
	{
	    lightCamera = GetComponent<Camera>();
        lightCamera.RenderWithShader(depthShader,"RenderType");
	}

    public void SetDepthShader(Shader shader)
    {
        depthShader = shader;
    }

}
