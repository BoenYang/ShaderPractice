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

    public Shader backFaceShader;


    private Material _mat;

    private Camera backCamera;


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

    private void RenderBackFace() {
        if (backCamera == null) {
            GameObject go = new GameObject("BackFaceCamera");
            Camera mainCamera = Camera.main;
            go.transform.SetParent(mainCamera.transform);
            go.transform.hideFlags = HideFlags.HideAndDontSave;
            backCamera = go.AddComponent<Camera>();
            backCamera.CopyFrom(mainCamera);
            backCamera.enabled = false;
            backCamera.clearFlags = CameraClearFlags.SolidColor;
            backCamera.backgroundColor = Color.white;
            backCamera.renderingPath = RenderingPath.Forward;
            backCamera.SetReplacementShader(backFaceShader, "RenderType");
            backCamera.targetTexture = GetBackFaceTexture();
        }
        backCamera.Render();
    }


    private RenderTexture backfacRT;

    private RenderTexture GetBackFaceTexture() {
        if (backfacRT == null) {
            backfacRT = new RenderTexture(Screen.width, Screen.height,24,RenderTextureFormat.RFloat);
            backfacRT.filterMode = FilterMode.Point;
        }
        return backfacRT;
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        RenderBackFace();
        mat.SetTexture("_BackfaceTex", GetBackFaceTexture());
        mat.SetMatrix("_WorldToView", GetComponent<Camera>().worldToCameraMatrix);
        Graphics.Blit(source, destination, mat,0);
    }


}
