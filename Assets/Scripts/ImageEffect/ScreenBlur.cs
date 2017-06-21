using UnityEngine;

[ExecuteInEditMode]
public class ScreenBlur : ImageEffectBase
{
    public float BlurStrength = 1.0f;

    [Range(1,5)]
    public int Iteration = 1;


    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        BeginBlur(source,Iteration,BlurStrength);
        Graphics.Blit(source,destination) ;
    }
}
