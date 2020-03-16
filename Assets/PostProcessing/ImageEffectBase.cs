using UnityEngine;

public class ImageEffectBase : MonoBehaviour
{
    public Shader BlurShader;

    protected Material blurMaterial
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


    protected void BeginBlur(RenderTexture source,int iterations,float blurStrength)
    {
        blurMaterial.SetFloat("_BlurStrength", blurStrength);
        RenderTexture r1 = source;
        RenderTexture r2 = null;
        for (int i = 0; i < iterations; i++)
        {
            r2 = RenderTexture.GetTemporary(r1.width,r1.height,0);
            Graphics.Blit(r1,r2,blurMaterial,0);
            Graphics.Blit(r2,r1,blurMaterial,1);
            RenderTexture.ReleaseTemporary(r2);
        }
    }

    protected void EndBlur()
    {

    }


}

