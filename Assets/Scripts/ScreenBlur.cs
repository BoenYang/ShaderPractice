using UnityEngine;

[ExecuteInEditMode]
public class ScreenBlur : MonoBehaviour
{
    public float BlurStrength = 1.0f;

    [Range(1,5)]
    public int Iteration = 1;

    [HideInInspector]
    public Shader BlurShader;

    private Material blurMaterial
    {
        get
        {
            if (_blurMaterial == null)
            {
                _blurMaterial = new Material(BlurShader);
            }
            _blurMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _blurMaterial;
        }
    }

    private Material _blurMaterial;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        blurMaterial.SetFloat("_BlurStrength",BlurStrength);
      
        RenderTexture t0 = RenderTexture.GetTemporary(source.width/2,source.height/2,0,RenderTextureFormat.Default);
        Graphics.Blit(source, t0);


        for (int i = 0; i < Iteration; i++)
        {
            RenderTexture t1 = RenderTexture.GetTemporary(t0.width, t0.height, 0, RenderTextureFormat.Default);
            Graphics.Blit(t0,t1,blurMaterial,0);

            RenderTexture.ReleaseTemporary(t0);

            t0 = t1;
            t1 = RenderTexture.GetTemporary(t0.width, t0.height, 0, RenderTextureFormat.Default);

            Graphics.Blit(t0, t1, blurMaterial, 1);

            RenderTexture.ReleaseTemporary(t0);
            t0 = t1;
        }

        Graphics.Blit(t0,destination);
        RenderTexture.ReleaseTemporary(t0);
    }
}
