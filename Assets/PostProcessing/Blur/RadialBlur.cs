using UnityEngine;

[ExecuteInEditMode]
public class RadialBlur : MonoBehaviour
{


    [Range(0,1)]
    public float SampleDistance = 0.5f;


    [Range(0,1)]
    public float SampleStrength = 0.5f;

    [HideInInspector]
    public Shader Blur;

    private Material blurMaterial
    {
        get
        {
            if (_blurMaterial == null)
            {
                _blurMaterial = new Material(Blur);
            }
            _blurMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _blurMaterial;
        }
    }

    private Material _blurMaterial;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        blurMaterial.SetFloat("_SampleDistance",SampleDistance);
        blurMaterial.SetFloat("_SampleStrength", SampleStrength);
        
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source,rt,blurMaterial,0);

        blurMaterial.SetTexture("_BlueTex",rt);
        Graphics.Blit(source, destination, blurMaterial,1);


        RenderTexture.ReleaseTemporary(rt);
    }
}
