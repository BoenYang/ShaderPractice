using UnityEngine;

[ExecuteInEditMode]
public class MotionBlur : MonoBehaviour
{
    public float BlurAmount = 0.5f;

    [HideInInspector]
    public Shader MotionBlurShader;

    private Material blurMaterial
    {
        get
        {
            if (_blurMaterial == null)
            {
                _blurMaterial = new Material(MotionBlurShader);
            }
            _blurMaterial.hideFlags = HideFlags.HideAndDontSave;
            return _blurMaterial;
        }
    }

    private Material _blurMaterial;

    private RenderTexture preframeRenderTexture;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (preframeRenderTexture == null || preframeRenderTexture.width != source.width ||
            preframeRenderTexture.height != source.height)
        {
            preframeRenderTexture = new RenderTexture(source.width, source.height, 0);
            preframeRenderTexture.hideFlags = HideFlags.HideAndDontSave;
            Graphics.Blit(source, preframeRenderTexture);
        }

        preframeRenderTexture.MarkRestoreExpected();

        blurMaterial.SetFloat("_BlurAmount",1.0f - BlurAmount);

        Graphics.Blit(source,preframeRenderTexture,blurMaterial);
        Graphics.Blit(preframeRenderTexture,destination);
    }

  
}
