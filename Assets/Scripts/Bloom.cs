using UnityEngine;

[ExecuteInEditMode]
public class Bloom : MonoBehaviour
{
    public float LuminanceThreshold = 0.5f;

    public Shader BloomShader;

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
        bloomMaterial.SetFloat("_BlurStrength",1.0f);

        int w = source.width/2;
        int h = source.height/2;

        RenderTexture bloomRt = RenderTexture.GetTemporary(w,h,0,RenderTextureFormat.Default);
        RenderTexture t1 = bloomRt;
        RenderTexture t2 = null;
        Graphics.Blit(source, bloomRt, bloomMaterial, 0);

        //迭代模糊
        for (int i = 0; i < BlurIteration; i++)
        {
            t2 = RenderTexture.GetTemporary(w, h, 0);
            Graphics.Blit(t1,t2,bloomMaterial,1);
            RenderTexture.ReleaseTemporary(t1);

            t1 = t2;

            t2 = RenderTexture.GetTemporary(w,h,0);
            Graphics.Blit(t1,t2,bloomMaterial,2);

            RenderTexture.ReleaseTemporary(t1);

            t1 = t2;
        }


        bloomRt = t2;
        bloomMaterial.SetTexture("_BloomTex",bloomRt);

        Graphics.Blit(source,destination,bloomMaterial,3);
        RenderTexture.ReleaseTemporary(bloomRt);
    }
}
