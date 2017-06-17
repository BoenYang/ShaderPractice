using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SSAO : MonoBehaviour
{

    public Shader SSAOShader;

    public float Distance = 1f;

    public float Intensity = 2f;

    public float Radius = 0.01f;

    public float DistanceCutoff = 150;


    private Material ssaoMaterial
    {
        get
        {
            if (_ssaoMaterial == null)
            {
                _ssaoMaterial = new Material(SSAOShader);
            }
            _ssaoMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _ssaoMaterial;
        }
    }

    private Material _ssaoMaterial;

    private Camera m_Camera;


    void OnEnable()
    {
        m_Camera = GetComponent<Camera>();

        m_Camera.depthTextureMode |= DepthTextureMode.Depth;
        m_Camera.depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        ssaoMaterial.SetMatrix("_CameraToWorldMatrix",m_Camera.cameraToWorldMatrix);
        ssaoMaterial.SetMatrix("_InverseViewProject",(m_Camera.projectionMatrix * m_Camera.worldToCameraMatrix).inverse);
        ssaoMaterial.SetFloat("_Distance", Distance);
        ssaoMaterial.SetFloat("_Intensity",Intensity);
        ssaoMaterial.SetFloat("_Radius",Radius);
        ssaoMaterial.SetFloat("_DistanceCutoff", DistanceCutoff);

        RenderTexture rt = RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.ARGB32);

        Graphics.Blit(rt,rt,ssaoMaterial,0);
        Graphics.Blit(source, rt, ssaoMaterial,1);
        Graphics.Blit(rt, destination);

        RenderTexture.ReleaseTemporary(rt);
    }
}
