using UnityEngine;

[ExecuteInEditMode]
public class ScreenBlur : MonoBehaviour
{
    public Shader BlurShader;

    public Shader DownSample;

    private Material downSampleMaterial
    {
        get
        {
            if (_downSampleMaterial == null)
            {
                _downSampleMaterial = new Material(DownSample);
            }
            _downSampleMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _downSampleMaterial;
        }
    }

    private Material _downSampleMaterial;


    private Material blurMaterial
    {
        get
        {
            if (_blurMaterial == null)
            {
                _blurMaterial = new Material(BlurShader);
            }
            _downSampleMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _blurMaterial;
        }
    }

    private Material _blurMaterial;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      
        RenderTexture downSampleText = RenderTexture.GetTemporary(source.width/2,source.height/2,0,RenderTextureFormat.Default);
        downSampleText.name = "downSample";
        Graphics.Blit(source, downSampleText,downSampleMaterial);

        RenderTexture temp = RenderTexture.GetTemporary(source.width/2,source.height/ 2,0, RenderTextureFormat.Default);
        temp.name = "t1";
        Graphics.Blit(downSampleText, temp, blurMaterial, 0);

        Graphics.Blit(temp, destination, blurMaterial,1);

        RenderTexture.ReleaseTemporary(temp);
        RenderTexture.ReleaseTemporary(downSampleText);
    }
}
