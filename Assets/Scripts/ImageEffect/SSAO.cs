using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SSAO : MonoBehaviour
{

    public Shader SSAOShader;


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

//    public void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//
//    }
}
