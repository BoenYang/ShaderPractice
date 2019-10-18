using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;


[ExecuteInEditMode]
public class SSR : MonoBehaviour
{

    [HideInInspector]
    public Shader shader;


    private Material _mat;


    private Material mat {
        get {
            if (_mat == null) {
                _mat = new Material(shader);
            }
            return _mat;
        }
    }

    private void Start() {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        mat.SetMatrix("_WorldToView", GetComponent<Camera>().worldToCameraMatrix);
        Graphics.Blit(source, destination, mat,0);
    }


}
