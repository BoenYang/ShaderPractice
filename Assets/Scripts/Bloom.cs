using UnityEngine;

[ExecuteInEditMode]
public class Bloom : MonoBehaviour
{
    public float LuminanceThreshold = 0.5f;

    public Shader BloomShader;

    private Material bloomMaterial
    {
        get
        {
            if (_bloomMaterial == null)
            {
                _bloomMaterial = new Material(BloomShader);
            }
            _bloomMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _bloomMaterial;
        }
    }

    private Material _bloomMaterial;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        bloomMaterial.SetFloat("_LuminanceThreshold", LuminanceThreshold);

        RenderTexture rt = RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.Default);
        rt.name = "BloomRT";
        Graphics.Blit(source, rt, bloomMaterial, 0);

        RenderTexture bloomRt1 = RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.Default);
        RenderTexture bloomRt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default);

        Graphics.Blit(rt,bloomRt1,bloomMaterial,1);
        Graphics.Blit(bloomRt1,bloomRt,bloomMaterial,2);

        bloomMaterial.SetTexture("_BloomTex",bloomRt);

        Graphics.Blit(source,destination,bloomMaterial,3);

        RenderTexture.ReleaseTemporary(rt);
        RenderTexture.ReleaseTemporary(bloomRt1);
        RenderTexture.ReleaseTemporary(bloomRt);
    }
}
