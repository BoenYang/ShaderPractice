using UnityEngine;

[ExecuteInEditMode]
public class Bloom : ImageEffectBase
{
    [Range(0.0f, 1.0f)]
    public float LuminanceThreshold = 0.5f;

    public Shader BloomShader;

    [Range(1, 5)]
    public int BlurIteration = 1;

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

        int w = source.width/2;
        int h = source.height/2;

        RenderTexture bloomRt = RenderTexture.GetTemporary(w,h,0,RenderTextureFormat.Default);

        Graphics.Blit(source, bloomRt, bloomMaterial, 0);
        
        BeginBlur(bloomRt,BlurIteration,1);

        bloomMaterial.SetTexture("_BloomTex",bloomRt);
        Graphics.Blit(source,destination,bloomMaterial,1);
        RenderTexture.ReleaseTemporary(bloomRt);
    }
}
