using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowMap : MonoBehaviour
{
    public GameObject SceneAABB;

    public RenderTexture ShadowMapRt;

    public Shader DepthShader;

    private Camera lightCamera;

	// Use this for initialization
	void Start () {
	    CreateShadowCamera();
	}

    private void CreateShadowCamera()
    {
        GameObject cameraGo = new GameObject("LightCamera");
        cameraGo.transform.SetParent(transform);
        cameraGo.transform.localPosition = Vector3.zero;
        cameraGo.transform.localRotation = Quaternion.identity;
        
        lightCamera = cameraGo.AddComponent<Camera>();
        lightCamera.orthographic = true;
        lightCamera.clearFlags = CameraClearFlags.Color;
        lightCamera.backgroundColor = Color.black;
        lightCamera.targetTexture = ShadowMapRt;
        lightCamera.enabled = false;


        Shader.SetGlobalTexture("_ShadowDepthMap",ShadowMapRt);
        SetFitToScene();

        CaptureDepth depthCapture = cameraGo.AddComponent<CaptureDepth>();
        depthCapture.SetDepthShader(DepthShader);

        Matrix4x4 posToUV = new Matrix4x4();
        posToUV.SetRow(0, new Vector4(0.5f, 0, 0, 0.5f));
        posToUV.SetRow(1, new Vector4(0, 0.5f, 0, 0.5f));
        posToUV.SetRow(2, new Vector4(0, 0, 1, 0));
        posToUV.SetRow(3, new Vector4(0, 0, 0, 1));


        Matrix4x4 worldToView = lightCamera.worldToCameraMatrix;
        Matrix4x4 projection = GL.GetGPUProjectionMatrix(lightCamera.projectionMatrix, false);

        Matrix4x4 m = posToUV * projection * worldToView;
        Shader.SetGlobalMatrix("_LightProjection", m);
    }

    private void SetFitToScene()
    {
        List<Vector3> sceneAABBVertex = new List<Vector3>();

        Matrix4x4 sceneLocalToWorldMat = SceneAABB.transform.localToWorldMatrix;

        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3(  0.5f,  0.5f,  0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3( -0.5f,  0.5f,  0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3(  0.5f, -0.5f,  0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3( -0.5f, -0.5f,  0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3(  0.5f,  0.5f, -0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3( -0.5f,  0.5f, -0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3(  0.5f, -0.5f, -0.5f)));
        sceneAABBVertex.Add(sceneLocalToWorldMat.MultiplyPoint(new Vector3( -0.5f, -0.5f, -0.5f)));

        BoundBox3D box = new BoundBox3D();
        sceneAABBVertex.ForEach((v)=> box.Update(transform.worldToLocalMatrix.MultiplyPoint(v)));

        lightCamera.transform.localPosition = new Vector3(box.CenterX,box.CenterY,0);
        lightCamera.orthographicSize = Mathf.Max(box.SizeX/2, box.SizeY/2);
        lightCamera.nearClipPlane = box.MinZ - lightCamera.transform.localPosition.z;
        lightCamera.farClipPlane = box.MaxZ - lightCamera.transform.localPosition.z;


    }
}

public class BoundBox3D
{
    public float MinX = float.MaxValue;

    public float MinY = float.MaxValue;

    public float MinZ = float.MaxValue;

    public float MaxX = float.MinValue;

    public float MaxY = float.MinValue;

    public float MaxZ = float.MinValue;

    public float CenterX {
        get { return (MinX + MaxX)/2; }
    }

    public float CenterY
    {
        get { return (MinY + MaxY) / 2; }
    }
    public float CenterZ
    {
        get { return (MinZ + MaxZ) / 2; }
    }

    public float SizeX
    {
        get { return MaxX - MinX; }
    }

    public float SizeY
    {
        get { return MaxY - MinY; }
    }
    public float SizeZ
    {
        get { return MaxZ - MinZ; }
    }


    public void UpdateX(float val)
    {
        MinX = Mathf.Min(MinX, val);
        MaxX = Mathf.Max(MaxX, val);
    }

    public void UpdateY(float val)
    {
        MinY = Mathf.Min(MinY, val);
        MaxY = Mathf.Max(MaxY, val);
    }

    public void UpdateZ(float val)
    {
        MinZ = Mathf.Min(MinZ, val);
        MaxZ = Mathf.Max(MaxZ, val);
    }

    public void Update(Vector3 val)
    {
        UpdateX(val.x);
        UpdateY(val.y);
        UpdateZ(val.z);
    }

}
